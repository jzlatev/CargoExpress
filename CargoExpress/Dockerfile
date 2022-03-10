#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CargoExpress/CargoExpress.csproj", "CargoExpress/"]
RUN dotnet restore "CargoExpress/CargoExpress.csproj"
COPY . .
WORKDIR "/src/CargoExpress"
RUN dotnet build "CargoExpress.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CargoExpress.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CargoExpress.dll"]