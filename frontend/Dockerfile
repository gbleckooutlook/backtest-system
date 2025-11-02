# Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy application files
COPY . .

# Build the application
RUN npm run build

# Runtime stage
FROM node:20-alpine AS runtime
WORKDIR /app

# Copy built application
COPY --from=build /app/.output /app/.output

EXPOSE 3000
ENV HOST=0.0.0.0
ENV PORT=3000

CMD ["node", ".output/server/index.mjs"]


