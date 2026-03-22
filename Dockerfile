FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Planpipe.Api/Planpipe.Api.csproj", "src/Planpipe.Api/"]
COPY ["src/Planpipe.Application/Planpipe.Application.csproj", "src/Planpipe.Application/"]
COPY ["src/Planpipe.Infrastructure/Planpipe.Infrastructure.csproj", "src/Planpipe.Infrastructure/"]
COPY ["src/Planpipe.Core/Planpipe.Core.csproj", "src/Planpipe.Core/"]
RUN dotnet restore "src/Planpipe.Api/Planpipe.Api.csproj"

COPY . .
WORKDIR "/src/src/Planpipe.Api"
RUN dotnet build "Planpipe.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Planpipe.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Planpipe.Api.dll"]
