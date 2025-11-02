# Sistema de Backtest CPGR

Sistema de backtest para operações no mercado financeiro (B3 e Forex) utilizando .NET 8, PostgreSQL, Nuxt 3 e PrimeVue.

## 🚀 Tecnologias

### Backend
- .NET 8 (ASP.NET Core Web API)
- PostgreSQL 15
- Dapper (ORM)
- CsvHelper (processamento de CSV)

### Frontend
- Nuxt 3
- Vue 3 (Composition API)
- PrimeVue (componentes UI)
- TypeScript

### Infraestrutura
- Docker & Docker Compose
- PostgreSQL com volumes persistentes

## 📋 Pré-requisitos

- Docker Desktop instalado e em execução
- Node.js 20+ (para desenvolvimento local)
- .NET 8 SDK (para desenvolvimento local)

## 🔧 Instalação e Execução

### Usando Docker Compose (Recomendado)

1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd backtest-system
```

2. Suba os containers
```bash
docker-compose up --build
```

3. Acesse as aplicações:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

### Desenvolvimento Local

#### Backend
```bash
cd backend
dotnet restore
dotnet run
```

#### Frontend
```bash
cd frontend
npm install
npm run dev
```

## 📊 Funcionalidades

### Ativos
- ✅ Cadastro de ativos (Mini-Dólar, etc.)
- ✅ Upload de arquivo CSV com candles
- ✅ Listagem paginada de ativos
- ✅ Suporte para mercados B3 e Forex
- ✅ Configuração de timeframes (5min, 15min, etc.)

### Backtest (Em desenvolvimento)
- 🔄 Criar backtest
- 🔄 Listar backtests
- 🔄 Análise de resultados

## 📁 Estrutura de Pastas

```
backtest-system/
├── backend/               # API .NET 8
│   ├── Controllers/       # Endpoints da API
│   ├── Services/          # Lógica de negócio
│   ├── Repositories/      # Acesso a dados (Dapper)
│   ├── Models/            # Entidades
│   ├── DTOs/              # Data Transfer Objects
│   └── Database/          # Scripts SQL
├── frontend/              # Aplicação Nuxt 3
│   ├── app/
│   │   ├── pages/        # Páginas da aplicação
│   │   └── layouts/      # Layouts
│   ├── composables/       # Composables Vue
│   └── plugins/           # Plugins Nuxt
└── docker-compose.yml     # Orquestração Docker
```

## 🗄️ Banco de Dados

### Tabelas

#### Ativos
- Id (PK)
- Nome
- Mercado (B3/Forex)
- Codigo
- Timeframe
- NomeArquivoCsv
- DataCriacao

#### Candles
- Id (PK)
- AtivoId (FK)
- Data
- Abertura
- Maxima
- Minima
- Fechamento
- ContadorCandles

## 📄 Formato do CSV

O arquivo CSV deve conter as seguintes colunas:

```
Data,Abertura,Máxima,Mínima,Fechamento,Contador de Candles
31/10/2025 18:20,152205,152240,152155,152225,113
31/10/2025 18:15,152185,152230,152160,152205,112
```

## 🔐 Autenticação (Próxima Fase)

- JWT Authentication
- Roles: Admin e Assinante

## 🛠️ Comandos Úteis

### Docker
```bash
# Parar containers
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Ver logs
docker-compose logs -f [service-name]

# Rebuild específico
docker-compose up --build [service-name]
```

### Backend
```bash
# Restaurar pacotes
dotnet restore

# Build
dotnet build

# Run
dotnet run

# Watch (hot reload)
dotnet watch run
```

### Frontend
```bash
# Instalar dependências
npm install

# Desenvolvimento
npm run dev

# Build para produção
npm run build

# Preview produção
npm run preview
```

## 📝 API Endpoints

### Ativos
- `GET /api/ativos` - Listar ativos (paginado)
- `POST /api/ativos` - Criar novo ativo com CSV
- `GET /api/ativos/{id}` - Obter ativo específico

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT.


