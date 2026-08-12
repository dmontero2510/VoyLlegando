@echo off
title VoyLlegando - Git Commit

cd /d C:\SIE\VoyLlegando\Desarrollo

echo.
echo ============================================
echo   VOYLLEGANDO - GIT COMMIT
echo ============================================
echo.

echo [1/5] Verificando repositorio...
git status

echo.
echo [2/5] Agregando archivos modificados...
git add .

echo.
echo [3/5] Archivos que entraran al commit...
git status

echo.
echo [4/5] Creando commit...
git commit -m "Admin cereales y registro de empresas"

if errorlevel 1 (
    echo.
    echo ATENCION:
    echo Git no pudo crear el commit o no habia cambios.
    echo.
    pause
    exit /b 1
)

echo.
echo [5/5] Commit creado correctamente.
echo.

git log -1 --oneline

echo.
echo ============================================
echo   COMMIT FINALIZADO
echo ============================================
echo.

pause