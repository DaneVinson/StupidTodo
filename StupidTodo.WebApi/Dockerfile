FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["StupidTodo.WebApi/StupidTodo.WebApi.csproj", "StupidTodoWebApi/"]
RUN dotnet restore "StupidTodo.WebApi/StupidTodo.WebApi.csproj"
COPY . .
WORKDIR "/src/StupidTodoWebApi"
RUN dotnet build "StupidTodo.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "StupidTodo.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "StupidTodo.WebApi.dll"]