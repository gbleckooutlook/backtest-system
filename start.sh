#!/bin/bash

echo "=========================================="
echo "  Backtest System CPGR - Inicialização"
echo "=========================================="
echo ""

# Verificar se Docker está rodando
if ! docker info > /dev/null 2>&1; then
  echo "❌ Docker não está rodando. Por favor, inicie o Docker Desktop."
  exit 1
fi

echo "✅ Docker está rodando"
echo ""

# Verificar se é a primeira execução
if [ ! "$(docker volume ls -q -f name=backtest-system_postgres_data)" ]; then
  echo "📦 Primeira execução detectada!"
  echo "🔧 O banco de dados será criado automaticamente..."
  echo ""
fi

# Subir os containers
echo "🚀 Subindo os containers..."
docker-compose up -d

echo ""
echo "⏳ Aguardando os serviços ficarem prontos..."
sleep 10

# Verificar status dos containers
if [ "$(docker ps -q -f name=backtest-postgres)" ] && \
   [ "$(docker ps -q -f name=backtest-backend)" ] && \
   [ "$(docker ps -q -f name=backtest-frontend)" ]; then
  echo ""
  echo "=========================================="
  echo "✅ Sistema iniciado com sucesso!"
  echo "=========================================="
  echo ""
  echo "🌐 Acesse:"
  echo "   Frontend: http://localhost:3001"
  echo "   Backend:  http://localhost:5001"
  echo ""
  echo "📋 Para ver os logs:"
  echo "   docker-compose logs -f"
  echo ""
  echo "🛑 Para parar:"
  echo "   docker-compose stop"
  echo ""
else
  echo ""
  echo "⚠️  Alguns containers falharam ao iniciar."
  echo "Execute 'docker-compose logs' para ver os erros."
  echo ""
fi

