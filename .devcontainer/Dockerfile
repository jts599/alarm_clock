FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install additional tools and utilities
RUN apt-get update && apt-get install -y \
    procps \
    iproute2 \
    iputils-ping \
    wireless-tools \
    sudo \
    tcpdump \
    netcat-openbsd \
    nmap \
    && rm -rf /var/lib/apt/lists/*

# Create workspace directory
WORKDIR /workspace

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true

# Expose ports
EXPOSE 5000
EXPOSE 3000

# Keep container running
CMD ["sleep", "infinity"]