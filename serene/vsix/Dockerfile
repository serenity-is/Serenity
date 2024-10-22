FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS serenity-base-env
RUN apt-get update && apt-get install -y npm

FROM serenity-base-env
WORKDIR /app

COPY .nupkg/ .nupkg/
RUN dotnet new install .nupkg/Serene.Templates*.nupkg
RUN dotnet new serene -n SRNTest

WORKDIR /app/SRNTest/SRNTest.Web
COPY docker-appsettings-sqlite.json ./appsettings.machine.json

EXPOSE 5002
ENTRYPOINT ["dotnet", "run", "--urls", "http://+:5002"]