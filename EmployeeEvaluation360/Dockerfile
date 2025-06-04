# Stage 1: Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
RUN apt-get update && apt-get install -y ca-certificates

# Stage 2: Build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["EmployeeEvaluation360/EmployeeEvaluation360.csproj", "EmployeeEvaluation360/"]
RUN dotnet restore "./EmployeeEvaluation360/EmployeeEvaluation360.csproj"
COPY . .
WORKDIR "/src/EmployeeEvaluation360"
RUN dotnet build "./EmployeeEvaluation360.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EmployeeEvaluation360.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeEvaluation360.dll"]