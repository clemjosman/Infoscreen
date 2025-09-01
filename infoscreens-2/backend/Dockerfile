FROM mcr.microsoft.com/dotnet/sdk:8.0 AS installer-env

RUN mkdir -p /home/site/wwwroot
ARG VESACT_NUGET_REGISTRY_ACCESS_TOKEN
ARG PROJECT_DIR
ARG IS_PROD

# Install and register Zscaler certificate
COPY zscaler-root-ca.crt /usr/local/share/ca-certificates/zscaler-root-ca.crt
RUN cat /usr/local/share/ca-certificates/zscaler-root-ca.crt >> /etc/ssl/certs/ca-certificates.crt

COPY . /src/infoscreens
RUN sed -i 's/%VESACT_NUGET_REGISTRY_ACCESS_TOKEN%/'"$VESACT_NUGET_REGISTRY_ACCESS_TOKEN"'/g' /src/infoscreens/nuget.config
WORKDIR /src/infoscreens/${PROJECT_DIR}
RUN dotnet publish *.csproj --output /home/site/wwwroot


# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated6.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0-slim
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Install and register Zscaler certificate
COPY zscaler-root-ca.crt /usr/local/share/ca-certificates/zscaler-root-ca.crt
RUN cat /usr/local/share/ca-certificates/zscaler-root-ca.crt >> /etc/ssl/certs/ca-certificates.crt

RUN if [ "$IS_PROD" = "false" ]; then \
    EXPOSE 80; \
fi
EXPOSE 443

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]
