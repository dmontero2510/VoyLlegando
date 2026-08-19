@echo off
title VoyLlegando - Git Commit y Push

cd /d C:\SIE\VoyLlegando\Desarrollo

echo.
echo ============================================
echo   VOYLLEGANDO - GIT COMMIT
echo ============================================
echo.

git status

echo.
set /p MENSAJE=Ingrese descripcion del commit: 

if "%MENSAJE%"=="" (
    echo.
    echo ERROR: Debe ingresar una descripcion.
    echo.
    pause
    exit /b 1
)

echo.
echo [1/4] Agregando cambios...
git add -A -- . ":(exclude)src/**/bin/**" ":(exclude)src/**/obj/**"

if errorlevel 1 (
    echo.
    echo ERROR: No se pudieron preparar los cambios.
    echo.
    pause
    exit /b 1
)

echo.
echo [2/4] Creando commit...
git commit -m "%MENSAJE%"

if errorlevel 1 (
    echo.
    echo No se pudo crear el commit.
    echo Puede que no haya cambios pendientes.
    echo.
    pause
    exit /b 1
)

echo.
echo [3/4] Subiendo a origin/main...
git push origin main

if errorlevel 1 (
    echo.
    echo ERROR AL HACER PUSH
    echo El commit quedo guardado localmente.
    echo.
    pause
    exit /b 1
)

echo.
echo [4/4] Estado final...
git status

echo.
echo ============================================
echo   TODO OK
echo ============================================
echo.

git log -1 --oneline

echo.
pause
