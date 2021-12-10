FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY /src .
RUN ls
WORKDIR server/Turing.Api
RUN dotnet restore Turing.Api.csproj
RUN dotnet publish Turing.Api.csproj -c Release -o /app

FROM endeveit/docker-jq AS publish
WORKDIR /app
COPY --from=build /app .
RUN jq '.ConnectionStrings.DbConnectionString = "Uid=postgres;Pwd=1234;Server=localhost;Port=5432;Database=Turing;Pooling=true;"' appsettings.json > tmp.$$.json && mv tmp.$$.json appsettings.json
RUN cat appsettings.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Turing.Api.dll"]
