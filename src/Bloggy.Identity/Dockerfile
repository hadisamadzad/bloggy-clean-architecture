FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-stage

WORKDIR /app
COPY . .
RUN dotnet publish Bloggy.Identity/Bloggy.Identity.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-stage /app/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "Bloggy.Identity.dll"]