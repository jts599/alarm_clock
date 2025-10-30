FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install Node.js 20.x for frontend development
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs

# Install additional tools and utilities
RUN apt-get update && apt-get install -y \
    procps \
    git \
    curl \
    wget \
    sudo \
    && rm -rf /var/lib/apt/lists/*

# Create workspace directory
WORKDIR /workspace

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV NODE_ENV=development

# Expose ports for backend and frontend
EXPOSE 5000
EXPOSE 3000

# Keep container running
CMD ["sleep", "infinity"]