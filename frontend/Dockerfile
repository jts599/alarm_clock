FROM node:16-alpine

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json (if available)
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the application for production
RUN npm run build

# Use a lightweight web server to serve the built app
FROM nginx:alpine
COPY --from=0 /app/build /usr/share/nginx/html

# Copy custom nginx configuration if needed
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose the application port
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]