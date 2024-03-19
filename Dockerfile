#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
WORKDIR /src
COPY ["Hanum.Recruit/Hanum.Recruit.csproj", "Hanum.Recruit/"]
RUN dotnet nuget add source --username $NUGET_USERNAME --password $NUGET_PASSWORD --store-password-in-clear-text --name hanum "https://nuget.pkg.github.com/hansei-hanum/index.json"
RUN dotnet restore "./Hanum.Recruit/Hanum.Recruit.csproj"
COPY . .
WORKDIR "/src/Hanum.Recruit"
RUN dotnet build "./Hanum.Recruit.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Hanum.Recruit.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hanum.Recruit.dll"]