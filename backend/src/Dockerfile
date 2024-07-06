# Use the official .NET SDK image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the working directory inside the container
WORKDIR /app

# Copy the remaining source code and build the application
COPY . .
WORKDIR /app/WebAPI

# To ensure that nuget.org DNS resolved.
RUN curl https://api.nuget.org/v3/index.json -k

RUN dotnet restore

RUN dotnet publish -c Release --output ../out

# Create a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV DOTNET_TieredPGO 1 
ENV DOTNET_TC_QuickJitForLoops 1 
ENV DOTNET_ReadyToRun 0

# Define the entry point for your application
ENTRYPOINT ["dotnet", "WebAPI.dll"]
