FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Identity.Worker/Identity.Worker.csproj"
RUN dotnet build "Identity.Worker/Identity.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.Worker/Identity.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD dotnet Identity.Worker.dll
