FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
# Run 'docker build' command from the solution directory
WORKDIR /app
COPY . .
RUN rm -rf ./src
COPY Identity Identity
RUN dotnet publish Identity -c release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT dotnet Identity.dll