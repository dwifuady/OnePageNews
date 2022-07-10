# OnePageNews
A news portal scrapper with an overkilled architecture. \
Just for learning purpose.

## How To Run
### Using dotnet run
```
cd .\src\Api\
dotnet run
```
### Using docker
```
docker build -t onepagenews .
docker run -dp 5000:5000 -e ASPNETCORE_URLS='http://+:5000' -e PORT='5000' onepagenews:latest
```

### Test the scrapper
```
curl -d '{"url":"https://news.detik.com/internasional/d-6171259/pm-sri-lanka-siap-mengundurkan-diri-usai-kediaman-presiden-diserbu-warga"}' -H "Content-Type: application/json" -X POST http://localhost:5000/parse
```