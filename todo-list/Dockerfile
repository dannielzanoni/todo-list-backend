FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["todo-list.csproj", "./"]
RUN dotnet restore "todo-list.csproj"

COPY . .
RUN dotnet publish "todo-list.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 5000

ENTRYPOINT ["dotnet", "todo-list.dll"]
