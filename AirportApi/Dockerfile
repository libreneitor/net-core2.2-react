FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir /code
WORKDIR /code
COPY . .
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
COPY --from=build /code/bin/Release/netcoreapp2.2/publish .
CMD dotnet AirportApi.dll