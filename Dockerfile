FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["LeaveManagement.csproj", "./"]
RUN dotnet restore "LeaveManagement.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LeaveManagement.csproj" -c $configuration -o /app/builde

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "LeaveManagement.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeaveManagement.dll"]