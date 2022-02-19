FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "TuringPracticalTest/Turing.sln"

RUN dotnet build "./TuringPracticalTest/src/Turing.Api/Turing.Api.csproj" -c Release -o /app/build
RUN dotnet build "./TuringPracticalTest/tests/Turing.Business.Tests/Turing.Business.Tests.csproj" -c Release -o /app/build

FROM build AS testrunner
WORKDIR "./TuringPracticalTest/tests/Turing.Business.Tests/"

CMD ["dotnet", "test"]