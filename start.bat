@echo off
chcp 65001 > nul

echo ==========================================
echo   Backtest System CPGR - Inicialização
echo ==========================================
echo.

REM Verificar se Docker está rodando
docker info > nul 2>&1
if %errorlevel% neq 0 (
  echo ❌ Docker não está rodando. Por favor, inicie o Docker Desktop.
  pause
  exit /b 1
)

echo ✅ Docker está rodando
echo.

REM Verificar se é a primeira execução
docker volume ls -q -f name=backtest-system_postgres_data > nul 2>&1
if %errorlevel% neq 0 (
  echo 📦 Primeira execução detectada!
  echo 🔧 O banco de dados será criado automaticamente...
  echo.
)

REM Subir os containers
echo 🚀 Subindo os containers...
docker-compose up -d

echo.
echo ⏳ Aguardando os serviços ficarem prontos...
timeout /t 10 /nobreak > nul

REM Verificar status dos containers
docker ps -q -f name=backtest-postgres > nul 2>&1
set postgres_running=%errorlevel%
docker ps -q -f name=backtest-backend > nul 2>&1
set backend_running=%errorlevel%
docker ps -q -f name=backtest-frontend > nul 2>&1
set frontend_running=%errorlevel%

if %postgres_running%==0 if %backend_running%==0 if %frontend_running%==0 (
  echo.
  echo ==========================================
  echo ✅ Sistema iniciado com sucesso!
  echo ==========================================
  echo.
  echo 🌐 Acesse:
  echo    Frontend: http://localhost:3001
  echo    Backend:  http://localhost:5001
  echo.
  echo 📋 Para ver os logs:
  echo    docker-compose logs -f
  echo.
  echo 🛑 Para parar:
  echo    docker-compose stop
  echo.
) else (
  echo.
  echo ⚠️  Alguns containers falharam ao iniciar.
  echo Execute 'docker-compose logs' para ver os erros.
  echo.
)

pause

