FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY *.sln ./
COPY samples/Zoeri.Azure.Graphs.Sample/Zoeri.Azure.Graphs.Sample.csproj samples/Zoeri.Azure.Graphs.Sample/
COPY source/Zoeri.Azure.Graphs/Zoeri.Azure.Graphs.csproj source/Zoeri.Azure.Graphs/
RUN dotnet restore
COPY . .
WORKDIR /src/samples/Zoeri.Azure.Graphs.Sample
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Zoeri.Azure.Graphs.Sample.dll"]
