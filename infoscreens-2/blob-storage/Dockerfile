FROM mcr.microsoft.com/azure-storage/azurite

# Install and register Zscaler certificate
COPY zscaler-root-ca.crt /usr/local/share/ca-certificates/zscaler-root-ca.crt
RUN cat /usr/local/share/ca-certificates/zscaler-root-ca.crt >> /etc/ssl/certs/ca-certificates.crt

# Install azure-storage-blob python package
RUN apk update && \
    apk --no-cache add py3-pip && \
    apk add --virtual=build gcc libffi-dev musl-dev python3-dev && \
    python3 -m venv /my-venv && \
    /my-venv/bin/pip3 install azure-storage-blob==12.12.0

# Copy init.py script
COPY ./init.py init.py

# Copy local blobs to azurite
COPY ./files files

# Run the blob emulator and initialize the blob containers
CMD /my-venv/bin/python3 init.py --directory=files & \
    azurite --blobHost 0.0.0.0 --blobPort 10000 --queueHost 0.0.0.0 --queuePort 10001 --tableHost 0.0.0.0 --tablePort 10002 -l data