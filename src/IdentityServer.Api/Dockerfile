FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "IdentityServer.Api/IdentityServer.Api.csproj"
RUN dotnet build "IdentityServer.Api/IdentityServer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer.Api/IdentityServer.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD dotnet IdentityServer.Api.dll
