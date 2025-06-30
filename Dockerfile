FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InventoryManagementSystem.sln", "./"]
COPY ["IMS/InventoryManagementSystem.csproj", "IMS/"]
RUN dotnet restore "IMS/InventoryManagementSystem.csproj"
COPY . .
WORKDIR "/src/IMS"
RUN dotnet build "InventoryManagementSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryManagementSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InventoryManagementSystem.dll"] 