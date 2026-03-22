using System.Text;
using Newtonsoft.Json.Linq;

public static class RemoteConfigurationExtensions
{
    public static IConfigurationBuilder AddRemoteConfig(
        this IConfigurationBuilder builder,
        string url
    )
    {
        return builder.Add(new RemoteConfigurationSource(url));
    }
}

public class RemoteConfigurationSource : IConfigurationSource
{
    public string Url { get; }

    public RemoteConfigurationSource(string url)
    {
        Url = url;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new RemoteConfigurationProvider(Url);
    }
}

public class RemoteConfigurationProvider : ConfigurationProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _url;
    private string? _etagBase;
    private string? _etagEnv;

    private JObject jsonBaseObj = new();
    private JObject jsonEnvObj = new();

    public RemoteConfigurationProvider(string url)
    {
        _url = url;
        _httpClient = new HttpClient();
    }

    public override void Load()
    {
        LoadAsync().GetAwaiter().GetResult();
        StartPolling();
    }

    private void StartPolling()
    {
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                try
                {
                    await LoadAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Config Load Failed: {ex.Message}");
                }
            }
        });
    }

    private async Task LoadAsync()
    {
        var uriBase = new UriBuilder(_url);
        uriBase.Path += "appsettings.json";

        var requestBase = new HttpRequestMessage(HttpMethod.Get, uriBase.Uri); 

        if (_etagBase is not null)
        {
            requestBase.Headers.IfNoneMatch.Add(
                new System.Net.Http.Headers.EntityTagHeaderValue(_etagBase)
            );
        }

        var responseBase = await _httpClient.SendAsync(requestBase);

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (env is null) return;

        var uriEnv = new UriBuilder(_url);
        uriEnv.Path += $"appsettings.{env}.json";

        var requestEnv = new HttpRequestMessage(HttpMethod.Get, uriEnv.Uri);

        if (_etagEnv is not null)
        {
            requestEnv.Headers.IfNoneMatch.Add(
                new System.Net.Http.Headers.EntityTagHeaderValue(_etagEnv)
            );
        }

        var responseEnv = await _httpClient.SendAsync(requestEnv);

        if (responseBase.StatusCode == System.Net.HttpStatusCode.NotModified && responseEnv.StatusCode == System.Net.HttpStatusCode.NotModified)
        {
            throw new Exception("Configs did not change!");
        }
        if (responseBase.StatusCode != System.Net.HttpStatusCode.NotModified)
        {
            responseBase.EnsureSuccessStatusCode();
            _etagBase = responseBase.Headers.ETag?.Tag;

            var jsonBase = await responseBase.Content.ReadAsStringAsync();

            jsonBaseObj = JObject.Parse(jsonBase);
        }
        if (responseEnv.StatusCode != System.Net.HttpStatusCode.NotModified)
        {
            responseEnv.EnsureSuccessStatusCode();
            _etagEnv = responseEnv.Headers.ETag?.Tag;

            var jsonEnv = await responseEnv.Content.ReadAsStringAsync();
            jsonEnvObj = JObject.Parse(jsonEnv);
        }

        JObject jsonObject = new JObject();

        jsonObject.Merge(jsonBaseObj, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });
        jsonObject.Merge(jsonEnvObj, new JsonMergeSettings() { MergeArrayHandling = MergeArrayHandling.Union });

        var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonObject.ToString()));

        var config = new ConfigurationBuilder().AddJsonStream(stream).Build();

        var data = config.AsEnumerable().Where(x => x.Value is not null).ToDictionary();

        Data = data ?? new Dictionary<string, string?>();

        OnReload();
    }
}
