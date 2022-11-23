#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Demo.Api/Demo.Api.csproj", "src/Demo.Api/"]
COPY ["src/Demo.Business/Demo.Business.csproj", "src/Demo.Business/"]
COPY ["src/Demo.Core/Demo.Core.csproj", "src/Demo.Core/"]
COPY ["src/Demo.Util/Demo.Util.csproj", "src/Demo.Util/"]
COPY ["src/Demo.Infrastructure/Demo.Infrastructure.csproj", "src/Demo.Infrastructure/"]
RUN dotnet restore "src/Demo.Api/Demo.Api.csproj"
COPY . .
WORKDIR "/src/src/Demo.Api"
RUN dotnet build "Demo.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Demo.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Demo.Api.dll"]