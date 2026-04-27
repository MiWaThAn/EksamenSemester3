FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8001
ENV ASPNETCORE_URLS=http://+:8001

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["EksamenSemester3.UI/EksamenSemester3.UI.csproj", "EksamenSemester3.UI/"]
COPY ["EksamenSemester3.API/EksamenSemester3.API.csproj", "EksamenSemester3.API/"]
COPY ["EksamenSemester3.Domain/EksamenSemester3.Domain.csproj", "EksamenSemester3.Domain/"]
COPY ["EksamenSemester3.Application/EksamenSemester3.Application.csproj", "EksamenSemester3.Application/"]
COPY ["EksamenSemester3.Infrastructure/EksamenSemester3.Infrastructure.csproj", "EksamenSemester3.Infrastructure/"]
COPY ["EksamenSemester3.Shared/EksamenSemester3.Shared.csproj", "EksamenSemester3.Shared/"]

RUN dotnet restore "EksamenSemester3.UI/EksamenSemester3.UI.csproj"


COPY ["EksamenSemester3.UI/", "EksamenSemester3.UI/"]
COPY ["EksamenSemester3.API/", "EksamenSemester3.API/"]
COPY ["EksamenSemester3.Domain/", "EksamenSemester3.Domain/"]
COPY ["EksamenSemester3.Application/", "EksamenSemester3.Application/"]
COPY ["EksamenSemester3.Infrastructure/", "EksamenSemester3.Infrastructure/"]
COPY ["EksamenSemester3.Shared/", "EksamenSemester3.Shared/"]
RUN dotnet build "EksamenSemester3.UI/EksamenSemester3.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "EksamenSemester3.UI/EksamenSemester3.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "EksamenSemester3.UI.dll"]