#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
#EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GloboTicket.Services.ExternalPayment/GloboTicket.Services.ExternalPayment.csproj", "GloboTicket.Services.ExternalPayment/"]
COPY ["GloboTicket.Integration.MessagingBus/GloboTicket.Integration.MessagingBus.csproj", "GloboTicket.Integration.MessagingBus/"]
COPY ["GloboTicket.Integration.Messages/GloboTicket.Integration.Messages.csproj", "GloboTicket.Integration.Messages/"]
RUN dotnet restore "./GloboTicket.Services.ExternalPayment/./GloboTicket.Services.ExternalPayment.csproj"
COPY . .
WORKDIR "/src/GloboTicket.Services.ExternalPayment"
RUN dotnet build "./GloboTicket.Services.ExternalPayment.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GloboTicket.Services.ExternalPayment.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GloboTicket.Services.ExternalPayment.dll"]