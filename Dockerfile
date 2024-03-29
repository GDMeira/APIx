# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy everything else and build app
COPY . .
RUN dotnet restore
WORKDIR /source
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 5045
ENV ASPNETCORE_URLS=http://+:5045
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=build /app ./

COPY /Monitor/startApix.sh /
RUN apt-get update && apt-get install -y netcat-openbsd
RUN chmod +x /startApix.sh

# Defina o script de inicialização como o ponto de entrada
ENTRYPOINT ["/startApix.sh"]