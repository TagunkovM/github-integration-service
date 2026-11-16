FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props GitHubIntegration.sln ./
COPY src/GitHubIntegration.Domain/GitHubIntegration.Domain.csproj src/GitHubIntegration.Domain/
COPY src/GitHubIntegration.Application/GitHubIntegration.Application.csproj src/GitHubIntegration.Application/
COPY src/GitHubIntegration.Infrastructure/GitHubIntegration.Infrastructure.csproj src/GitHubIntegration.Infrastructure/
COPY src/GitHubIntegration.Api/GitHubIntegration.Api.csproj src/GitHubIntegration.Api/
COPY tests/GitHubIntegration.UnitTests/GitHubIntegration.UnitTests.csproj tests/GitHubIntegration.UnitTests/
COPY tests/GitHubIntegration.IntegrationTests/GitHubIntegration.IntegrationTests.csproj tests/GitHubIntegration.IntegrationTests/

RUN dotnet restore GitHubIntegration.sln

COPY src/ src/
COPY tests/ tests/
RUN dotnet publish src/GitHubIntegration.Api/GitHubIntegration.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "GitHubIntegration.Api.dll"]
