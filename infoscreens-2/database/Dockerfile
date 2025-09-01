# Choose ubuntu version
FROM mcr.microsoft.com/mssql/server:latest

# Create app directory
WORKDIR /usr/src/app

USER root
# Install and register Zscaler certificate
COPY zscaler-root-ca.crt /usr/local/share/ca-certificates/zscaler-root-ca.crt
RUN cat /usr/local/share/ca-certificates/zscaler-root-ca.crt >> /etc/ssl/certs/ca-certificates.crt

# Install tool to ensure scripts are working
RUN apt-get update
RUN apt-get install dos2unix -y
USER mssql

# Copy initialization scripts
COPY . /usr/src/app

USER root
RUN dos2unix /usr/src/app/entrypoint.sh
RUN dos2unix /usr/src/app/healthcheck.sh
RUN dos2unix /usr/src/app/run-initialization.sh
USER mssql

# Set environment variables, not have to write them with the docker run command
# Note: make sure that your password matches what is in the run-initialization script 
ENV SA_PASSWORD=password123!
ENV ACCEPT_EULA=Y

# Expose port 1433 in case accessing from other container
EXPOSE 1433

# Run Microsoft SQL Server and initialization script (at the same time)
CMD /bin/bash ./entrypoint.sh
