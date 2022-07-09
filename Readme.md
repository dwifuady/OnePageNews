# OnePageNews
A news portal scrapper with an overkilled architecture. \
Just for learning purpose.

## How To Run
### Using dotnet run
```
> cd .\src\Api\
> dotnet run
```
### Using docker
```
docker build -t onepagenews .

docker run -dp 5000:5000 -e ASPNETCORE_URLS='http://+:5000' -e PORT='5000' onepagenews:latest
```