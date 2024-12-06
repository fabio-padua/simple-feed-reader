# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ./SimpleFeedReader/*.csproj ./SimpleFeedReader/
WORKDIR /src/SimpleFeedReader
RUN dotnet restore

# Copy the remaining files and build the application
COPY ./SimpleFeedReader/. ./
RUN dotnet publish --configuration Release --output /app/out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /app/out .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "SimpleFeedReader.dll"]