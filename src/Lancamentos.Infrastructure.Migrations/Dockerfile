FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["NuGet.Config", "./"]
COPY ["src/Lancamentos.Infrastructure.Migrations/Lancamentos.Infrastructure.Migrations.csproj", "src/Lancamentos.Infrastructure.Migrations/"]
RUN dotnet restore "src/Lancamentos.Infrastructure.Migrations/Lancamentos.Infrastructure.Migrations.csproj"

COPY . .
RUN dotnet publish "src/Lancamentos.Infrastructure.Migrations/Lancamentos.Infrastructure.Migrations.csproj" \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Lancamentos.Infrastructure.Migrations.dll"]
