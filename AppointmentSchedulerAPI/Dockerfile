FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /App

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /App
# COPY .env .env
COPY --from=build /App/out .

ENTRYPOINT ["dotnet", "AppointmentSchedulerAPI.dll"]

