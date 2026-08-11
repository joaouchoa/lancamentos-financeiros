FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NuGet.Config", "./"]
COPY ["src/Lancamentos.Api/Lancamentos.Api.csproj",                       "src/Lancamentos.Api/"]
COPY ["src/Lancamentos.Application/Lancamentos.Application.csproj",       "src/Lancamentos.Application/"]
COPY ["src/Lancamentos.Domain/Lancamentos.Domain.csproj",                 "src/Lancamentos.Domain/"]
COPY ["src/Lancamentos.Infrastructure/Lancamentos.Infrastructure.csproj", "src/Lancamentos.Infrastructure/"]
COPY ["src/Shared.Contracts/Shared.Contracts.csproj",                     "src/Shared.Contracts/"]
RUN dotnet restore "src/Lancamentos.Api/Lancamentos.Api.csproj"

COPY . .
RUN dotnet publish "src/Lancamentos.Api/Lancamentos.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Lancamentos.Api.dll"]
