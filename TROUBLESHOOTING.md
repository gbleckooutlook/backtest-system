# 🔧 Troubleshooting - Backtest System

## Problemas Comuns e Soluções

### ❌ Docker não está rodando

**Erro**: `Cannot connect to the Docker daemon`

**Solução**: 
- Inicie o Docker Desktop
- Aguarde até o ícone do Docker ficar verde
- Execute novamente

---

### ❌ Porta já está em uso

**Erro**: `Bind for 0.0.0.0:3001 failed: port is already allocated`

**Solução**:
```bash
# Descobrir qual processo está usando a porta
netstat -ano | findstr :3001

# Matar o processo (substitua PID pelo número)
taskkill /PID <PID> /F

# Ou altere a porta no docker-compose.yml
```

---

### ❌ Banco de dados não criado

**Sintoma**: Erro 42P01 ao criar ativo

**Solução**:
```bash
# Remover o volume do banco e recriar
docker-compose down
docker volume rm backtest-system_postgres_data
docker-compose up -d

# Verificar se o script foi executado
docker-compose logs postgres | findstr "inicializado"
```

---

### ❌ Backend não consegue conectar ao banco

**Erro**: `Connection refused` ou `timeout`

**Solução**:
```bash
# Verificar se o PostgreSQL está saudável
docker ps

# Ver logs do banco
docker-compose logs postgres

# Reiniciar apenas o backend
docker-compose restart backend
```

---

### ❌ Frontend não carrega

**Sintoma**: Página em branco ou erro de conexão

**Solução**:
```bash
# Ver logs do frontend
docker-compose logs frontend

# Rebuild do frontend
docker-compose up -d --build frontend

# Limpar cache do navegador
# Abrir navegador em modo anônimo
```

---

### ❌ "Cannot find module" no frontend

**Solução**:
```bash
# Rebuild completo do frontend
docker-compose stop frontend
docker-compose rm -f frontend
docker-compose up -d --build frontend
```

---

### 🔄 Reset Completo do Sistema

Se nada funcionar, faça um reset completo:

```bash
# Parar tudo
docker-compose down

# Remover volumes
docker volume rm backtest-system_postgres_data

# Limpar imagens antigas (opcional)
docker-compose build --no-cache

# Subir tudo novamente
docker-compose up -d
```

---

### 📊 Comandos Úteis para Diagnóstico

```bash
# Ver todos os containers
docker ps -a

# Ver volumes
docker volume ls

# Ver logs de todos os serviços
docker-compose logs

# Ver logs em tempo real
docker-compose logs -f

# Executar comandos dentro do container do PostgreSQL
docker exec -it backtest-postgres psql -U postgres -d backtestdb

# Verificar tabelas criadas
docker exec -it backtest-postgres psql -U postgres -d backtestdb -c "\dt"

# Ver dados de uma tabela
docker exec -it backtest-postgres psql -U postgres -d backtestdb -c "SELECT * FROM Ativos;"
```

---

### 🐛 Ainda com problemas?

1. Verifique se tem espaço em disco suficiente
2. Verifique se o antivírus não está bloqueando o Docker
3. Reinicie o Docker Desktop
4. Reinicie o computador
5. Verifique os logs detalhados: `docker-compose logs > logs.txt`

---

### 📝 Informações do Sistema

Para reportar um problema, inclua:
- Versão do Docker: `docker --version`
- Sistema Operacional
- Logs dos containers: `docker-compose logs`
- Comando executado que causou o erro

