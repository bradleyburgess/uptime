FROM mcr.microsoft.com/dotnet/sdk:10.0-resolute-aot AS build
WORKDIR /src

COPY Uptime.Web/Uptime.Web.csproj Uptime.Web/
RUN dotnet restore Uptime.Web/Uptime.Web.csproj

COPY Uptime.Web/ Uptime.Web/
RUN dotnet publish Uptime.Web/Uptime.Web.csproj -c Release -o /app --no-restore

# Writable data dir for the non-root user (no shell in the chiseled image)
RUN mkdir /data

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-resolute-chiseled AS final
WORKDIR /app

COPY --from=build /app/ .
COPY --from=build --chown=$APP_UID:$APP_UID /data /data

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ConnectionStrings__Sqlite="Data Source=/data/app.db"

VOLUME /data
EXPOSE 8080
USER $APP_UID

ENTRYPOINT ["./Uptime.Web"]
