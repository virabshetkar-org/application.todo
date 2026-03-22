using Microsoft.AspNetCore.Mvc;

namespace Service.Todo.Controllers;

[ApiController]
[Route("/api/todo/[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ConfigurationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet(Name = "GetConfiguration")]
    public Dictionary<string, string?> Get()
    {
        return _configuration.AsEnumerable().Where(item => item.Value is not null).ToDictionary();
    }
}
