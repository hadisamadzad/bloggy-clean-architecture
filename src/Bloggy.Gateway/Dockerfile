FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-stage

WORKDIR /app
COPY . .
RUN dotnet publish Bloggy.Gateway/Bloggy.Gateway.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-stage /app/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Bloggy.Gateway.dll"]