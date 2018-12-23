# Source of original Dockerfile: https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/

# Run from the solution directory
# docker build -t dane:test . --file StupidTodo.WebApi/Dockerfile

# Build image
FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ./StupidTodo.sln ./
COPY ./nuget.config ./

# Copy requried projects and dotnet restore
COPY ./StupidTodo.Domain/StupidTodo.Domain.csproj ./StupidTodo.Domain/StupidTodo.Domain.csproj
COPY ./StupidTodo.WebApi/StupidTodo.WebApi.csproj ./StupidTodo.WebApi/StupidTodo.WebApi.csproj
RUN dotnet restore
COPY ./ ./src

# Build and publish
RUN dotnet build -c Release --no-restore
RUN dotnet publish "./StupidTodo.WebApi/StupidTodo.WebApi.csproj" -c Release -o "../dist" --no-restore

# Create image
FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT Local
ENTRYPOINT ["dotnet", "StupidTodo.WebApi.dll"]
COPY --from=build /dist .

#FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
# Works for StupidTo.WebApi only, fails to build dependend projects
#FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
#WORKDIR /src
#COPY StupidTodo.WebApi.csproj ./
#COPY nuget.config ./
#RUN "dotnet restore --configfile nuget.config"
#COPY . ./
#WORKDIR /src
#RUN dotnet build StupidTodo.WebApi.csproj -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish StupidTodo.WebApi.csproj -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "StupidTodo.WebApi.dll"]

# Like VS generates
#FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
#WORKDIR /src
#COPY StupidTodo.WebApi/StupidTodo.WebApi.csproj StupidTodo.WebApi
#RUN "dotnet restore StupidTodo.WebApi/StupidTodo.WebApi.csproj --configFile nuget.config"
#COPY . .
#WORKDIR "/src/StupidTodo.WebApi"
#RUN dotnet build "StupidTodo.WebApi.csproj" -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish "StupidTodo.WebApi.csproj" -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "StupidTodo.WebApi.dll"]
#