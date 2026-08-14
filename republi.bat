iisreset /stop
cd C:\SIE\VoyLlegando\Desarrollo
dotnet publish src\Backend\VoyLlegando.Api\VoyLlegando.Api.csproj -c Release -o C:\SIE\VoyLlegando\Publicacion
iisreset /start
pause