FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /app
EXPOSE 5000
COPY . .
RUN dotnet restore
RUN dotnet publish -o /app/published-app

FROM mcr.microsoft.com/dotnet/sdk:6.0 as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
ENV ASPNETCORE_URLS http://+:5000
ENV PORT 5000
CMD [ "dotnet", "/app/Api.dll" ]