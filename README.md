# Backtest System - CPGR

Sistema de backtest para análise de ativos financeiros.

## 🚀 Como Rodar o Projeto

### Pré-requisitos

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Primeira Execução

1. **Clone o repositório** (se ainda não fez)
   ```bash
   git clone <url-do-repositorio>
   cd backtest-system
   ```

2. **Suba os containers**
   
   **Opção 1 - Script automático (Recomendado):**
   - **Windows**: Clique duas vezes em `start.bat` ou execute:
     ```cmd
     start.bat
     ```
   - **Linux/Mac**: Execute:
     ```bash
     chmod +x start.sh
     ./start.sh
     ```

   **Opção 2 - Manual:**
   ```bash
   docker-compose up -d
   ```

   Isso irá:
   - ✅ Criar o banco de dados PostgreSQL
   - ✅ Executar automaticamente os scripts de criação das tabelas
   - ✅ Subir o backend (API .NET 8)
   - ✅ Subir o frontend (Nuxt 3)

3. **Acesse a aplicação**
   - Frontend: [http://localhost:3001](http://localhost:3001)
   - Backend API: [http://localhost:5001](http://localhost:5001)

### Comandos Úteis

```bash
# Parar os containers
docker-compose stop

# Parar e remover os containers
docker-compose down

# Ver logs dos containers
docker-compose logs -f

# Ver logs de um container específico
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f postgres

# Rebuild de um container específico
docker-compose up -d --build backend
docker-compose up -d --build frontend

# Limpar tudo (containers, volumes, imagens)
docker-compose down -v
```

### Verificar se o Banco foi Criado

Para verificar se as tabelas foram criadas corretamente:

```bash
# Conectar ao PostgreSQL
docker exec -it backtest-postgres psql -U postgres -d backtestdb

# Dentro do psql, listar tabelas
\dt

# Ver estrutura da tabela Ativos
\d Ativos

# Ver estrutura da tabela Candles
\d Candles

# Sair do psql
\q
```

### Resetar o Banco de Dados

Se precisar resetar o banco de dados completamente:

```bash
# Parar os containers
docker-compose down

# Remover o volume do banco de dados
docker volume rm backtest-system_postgres_data

# Subir novamente (irá recriar o banco do zero)
docker-compose up -d
```

> **Nota**: O script de inicialização (`docker/postgres/init.sql`) só é executado quando o banco é criado pela primeira vez. Se o volume já existe, o script não será executado novamente.

## 📁 Estrutura do Projeto

```
backtest-system/
├── backend/                    # API .NET 8
│   ├── Controllers/           # Controladores da API
│   ├── Services/              # Lógica de negócio
│   ├── Repositories/          # Acesso ao banco de dados
│   └── Models/                # Modelos de dados
├── frontend/                   # Nuxt 3 + Buefy
│   ├── app/
│   │   ├── pages/            # Páginas da aplicação
│   │   ├── components/       # Componentes Vue
│   │   ├── composables/      # Composables (useAtivos, etc)
│   │   ├── layouts/          # Layouts da aplicação
│   │   └── plugins/          # Plugins (Buefy)
│   └── nuxt.config.ts        # Configuração do Nuxt
├── docker/
│   └── postgres/
│       └── init.sql          # Script de inicialização do banco
├── docker-compose.yml        # Configuração dos containers
├── start.bat                 # Script de inicialização (Windows)
├── start.sh                  # Script de inicialização (Linux/Mac)
├── README.md                 # Este arquivo
└── TROUBLESHOOTING.md        # Guia de solução de problemas
```

## 🎯 Funcionalidades

- ✅ Criar ativos (ações, forex, etc.)
- ✅ Upload de arquivo CSV com dados históricos (candles)
- ✅ Listar ativos com paginação
- ✅ Editar ativos
- ✅ Deletar ativos (com exclusão em cascata dos candles)
- ✅ Interface dark theme moderna

## 📊 Formato do CSV

O arquivo CSV deve conter as seguintes colunas:

```
Data, Abertura, Máxima, Mínima, Fechamento, Contador de Candles
```

Exemplo:
```csv
2025-01-01 09:00:00,5000.00,5050.00,4990.00,5025.00,1
2025-01-01 09:05:00,5025.00,5060.00,5020.00,5055.00,2
```

## 🛠️ Tecnologias

- **Backend**: .NET 8, ASP.NET Core, Dapper, PostgreSQL
- **Frontend**: Nuxt 3, Vue 3, Buefy, TypeScript
- **Database**: PostgreSQL 15
- **Containerização**: Docker, Docker Compose

## 📝 Observações

- ✅ O banco de dados é criado automaticamente na primeira execução
- ✅ As tabelas são criadas automaticamente pelo script `docker/postgres/init.sql`
- ✅ Os dados persistem no volume Docker `postgres_data`
- ✅ Para resetar os dados, remova o volume e suba novamente os containers
- ✅ O script de inicialização só roda na primeira vez (quando o banco é criado)

## ❓ Problemas?

Consulte o guia de troubleshooting: [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

Problemas comuns:
- Porta já em uso
- Docker não está rodando
- Banco de dados não foi criado
- Frontend não carrega
