@echo off

echo ==============================
echo DETENIENDO IIS
echo ==============================

iisreset /stop

echo.
echo ==============================
echo COMPILANDO
echo ==============================

cd /d C:\SIE\VoyLlegando\Desarrollo\src\Backend\VoyLlegando.Api

dotnet build -c Release

if errorlevel 1 (
    echo.
    echo ERROR EN BUILD
    pause
    exit /b 1
)

echo.
echo ==============================
echo PUBLICANDO
echo ==============================

dotnet publish -c Release -o C:\SIE\VoyLlegando\Publicacion

if errorlevel 1 (
    echo.
    echo ERROR EN PUBLICACION
    pause
    exit /b 1
)

echo.
echo ==============================
echo INICIANDO IIS
echo ==============================

iisreset /start

echo.
echo ==============================
echo FINALIZADO CORRECTAMENTE
echo ==============================

pause