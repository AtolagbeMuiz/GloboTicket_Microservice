#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
#EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GloboTicket.Services.Ordering/GloboTicket.Services.Ordering.csproj", "GloboTicket.Services.Ordering/"]
RUN dotnet restore "./GloboTicket.Services.Ordering/./GloboTicket.Services.Ordering.csproj"
COPY . .
WORKDIR "/src/GloboTicket.Services.Ordering"
RUN dotnet build "./GloboTicket.Services.Ordering.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GloboTicket.Services.Ordering.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GloboTicket.Services.Ordering.dll"]