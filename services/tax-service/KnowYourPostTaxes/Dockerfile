FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["../KnowYourPostTaxes/KnowYourPostTaxes.csproj", "KnowYourPostTaxes/"]
COPY ["../KnowYourPostTaxes.Data/KnowYourPostTaxes.Data.csproj", "KnowYourPostTaxes.Data/"]
RUN dotnet restore "KnowYourPostTaxes/KnowYourPostTaxes.csproj"
COPY . .
WORKDIR "/src/KnowYourPostTaxes"
RUN dotnet build "KnowYourPostTaxes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KnowYourPostTaxes.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KnowYourPostTaxes.dll"]
