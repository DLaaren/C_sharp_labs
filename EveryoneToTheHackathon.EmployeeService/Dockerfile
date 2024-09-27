FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y curl
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EveryoneToTheHackathon.EmployeeService/EveryoneToTheHackathon.EmployeeService.csproj", "EveryoneToTheHackathon.EmployeeService/"]
RUN dotnet restore "EveryoneToTheHackathon.EmployeeService/EveryoneToTheHackathon.EmployeeService.csproj"
COPY . .
WORKDIR "/src/EveryoneToTheHackathon.EmployeeService"
RUN dotnet build "EveryoneToTheHackathon.EmployeeService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EveryoneToTheHackathon.EmployeeService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EveryoneToTheHackathon.EmployeeService.dll"]
