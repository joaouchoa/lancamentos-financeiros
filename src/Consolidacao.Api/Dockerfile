FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NuGet.Config", "./"]
COPY ["src/Consolidacao.Api/Consolidacao.Api.csproj",                       "src/Consolidacao.Api/"]
COPY ["src/Consolidacao.Application/Consolidacao.Application.csproj",       "src/Consolidacao.Application/"]
COPY ["src/Consolidacao.Domain/Consolidacao.Domain.csproj",                 "src/Consolidacao.Domain/"]
COPY ["src/Consolidacao.Infrastructure/Consolidacao.Infrastructure.csproj", "src/Consolidacao.Infrastructure/"]
COPY ["src/Shared.Contracts/Shared.Contracts.csproj",                       "src/Shared.Contracts/"]
RUN dotnet restore "src/Consolidacao.Api/Consolidacao.Api.csproj"

COPY . .
RUN dotnet publish "src/Consolidacao.Api/Consolidacao.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Consolidacao.Api.dll"]
