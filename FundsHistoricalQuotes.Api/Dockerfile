FROM microsoft/dotnet:2.1-aspnetcore-runtime-stretch-slim-arm32v7 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["FundsHistoricalQuotes.Api/FundsHistoricalQuotes.Api.csproj", "FundsHistoricalQuotes.Api/"]
COPY ["FundsHistoricalQuotes.Core/FundsHistoricalQuotes.Core.csproj", "FundsHistoricalQuotes.Core/"]
RUN dotnet restore "FundsHistoricalQuotes.Api/FundsHistoricalQuotes.Api.csproj"
COPY . .
WORKDIR "/src/FundsHistoricalQuotes.Api"
RUN dotnet build "FundsHistoricalQuotes.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FundsHistoricalQuotes.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FundsHistoricalQuotes.Api.dll"]