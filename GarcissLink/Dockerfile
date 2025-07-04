# Establece la imagen base
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Establece el directorio de trabajo
WORKDIR /app

# Copia los archivos del proyecto y restaura las dependencias
COPY . .
RUN dotnet restore

# Compila la aplicación
RUN dotnet publish -c Release -o out
RUN dotnet dev-certs https -ep /app/cert.pfx -p password

# Configura la imagen de producción
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
COPY --from=build /app/cert.pfx .
RUN rm -rf appsettings.json

# Instalamos curl para el healthcheck de Docker
RUN apt update && apt install -y curl

EXPOSE 5000

# Inicia la aplicación
ENTRYPOINT ["dotnet", "GarcissLink.dll"]