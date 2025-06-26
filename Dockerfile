# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only csproj file to restore dependencies
COPY EmployeeEvaluation360/EmployeeEvaluation360.csproj EmployeeEvaluation360/
RUN dotnet restore EmployeeEvaluation360/EmployeeEvaluation360.csproj

# Copy all files
COPY . .

WORKDIR /src/EmployeeEvaluation360
RUN dotnet build EmployeeEvaluation360.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish EmployeeEvaluation360.csproj -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeEvaluation360.dll"]
