# Source of original Dockerfile: https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/

# Run from the solution directory, i.e. directory that holds StupidTodo.sln
# docker build -t dane:stupidtodo . --file StupidTodo.WebApi/Dockerfile
# docker run -d -p 8080:80 --name stupidtodo dane:stupidtodo
# Windows nanoserver images won't work correctly using localhost, use the following to get container IP 
#	docker inspect -f "{{ .NetworkSettings.Networks.nat.IPAddress }}" stupidtodo

# Cleanup: docker system prune

# Windows build image
#FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
## Linux build image
FROM microsoft/dotnet:2.2.100-sdk-stretch AS build

WORKDIR ./
COPY ./StupidTodo.sln ./
COPY ./nuget.config ./

# Copy requried projects and dotnet restore
COPY ./StupidTodo.Domain/StupidTodo.Domain.csproj ./StupidTodo.Domain/StupidTodo.Domain.csproj
COPY ./StupidTodo.WebApi/StupidTodo.WebApi.csproj ./StupidTodo.WebApi/StupidTodo.WebApi.csproj
RUN dotnet restore
COPY ./ ./

# Build and publish
RUN dotnet build -c Release
RUN dotnet publish "./StupidTodo.WebApi/StupidTodo.WebApi.csproj" -c Release -o "../dist" --no-restore

# Windows runtime image
#FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803
# Linux runtime image
FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-stretch-slim
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Local
ENTRYPOINT ["dotnet", "StupidTodo.WebApi.dll"]
COPY --from=build /dist .
