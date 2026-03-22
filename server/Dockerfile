# =============================
# BASE RUNTIME
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS debug
WORKDIR /app
EXPOSE 8080

ENV COMPlus_ReadyToRun=0
ENV COMPlus_ZapDisable=1

RUN apt-get update && apt-get install -y unzip curl tini

RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

ENTRYPOINT ["/usr/bin/tini", "--", "dotnet"]

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base

WORKDIR /app

COPY *.csproj . 
RUN dotnet restore

COPY . . 

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS deploy

WORKDIR /app

COPY --from=base /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Service.Todo.dll"]
