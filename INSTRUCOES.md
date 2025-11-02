# 🚀 Sistema Backtest CPGR - Instruções de Uso

## ✅ Sistema Pronto e Funcionando!

Todos os serviços estão rodando com sucesso nos containers Docker.

## 📍 Acesso ao Sistema

### Frontend (Interface Web)
- **URL**: http://localhost:3001
- Aqui você vai cadastrar ativos e visualizar a listagem

### Backend (API)
- **URL API**: http://localhost:5001
- **Swagger**: http://localhost:5001/swagger
- Documentação interativa da API

### Banco de Dados
- **PostgreSQL**: localhost:5432
- **Database**: backtestdb
- **User**: postgres
- **Password**: postgres123

## 🎯 Como Usar o Sistema

### 1. Cadastrar um Ativo

1. Acesse http://localhost:3001
2. No menu superior, clique em **Ativo → Criar**
3. Preencha o formulário:
   - **Nome**: Ex: Mini-Dólar
   - **Mercado**: Escolha B3 ou Forex
   - **Código**: Ex: WDO
   - **Timeframe**: Ex: 5 minutos
   - **Arquivo CSV**: Faça upload do arquivo `exemplo-candles.csv` que está na raiz do projeto

4. Clique em **Salvar**
5. Você verá uma mensagem de sucesso e será redirecionado para a listagem

### 2. Visualizar Ativos

1. No menu, clique em **Ativo → Listar**
2. Você verá todos os ativos cadastrados em uma tabela
3. A tabela mostra:
   - Nome do Ativo
   - Mercado
   - Código
   - Timeframe
   - Data de Criação
4. Use os botões de paginação para navegar entre páginas

## 📊 Arquivo CSV de Exemplo

O arquivo `exemplo-candles.csv` está na raiz do projeto com dados de exemplo.

**Formato do CSV:**
```csv
Data,Abertura,Máxima,Mínima,Fechamento,Contador de Candles
31/10/2025 18:20,152205,152240,152155,152225,113
31/10/2025 18:15,152185,152230,152160,152205,112
...
```

## 🔧 Comandos Úteis

### Ver Status dos Containers
```bash
docker-compose ps
```

### Ver Logs
```bash
# Todos os serviços
docker-compose logs -f

# Apenas backend
docker-compose logs -f backend

# Apenas frontend
docker-compose logs -f frontend

# Apenas postgres
docker-compose logs -f postgres
```

### Parar os Containers
```bash
docker-compose down
```

### Parar e Remover Dados (cuidado!)
```bash
docker-compose down -v
```

### Reiniciar os Containers
```bash
docker-compose restart
```

### Rebuild e Reiniciar
```bash
docker-compose up --build -d
```

## 🗄️ Estrutura do Banco de Dados

### Tabela: Ativos
- `Id` - Identificador único
- `Nome` - Nome do ativo
- `Mercado` - B3 ou Forex
- `Codigo` - Código do ativo (ex: WDO)
- `Timeframe` - Período dos candles
- `NomeArquivoCsv` - Nome do arquivo enviado
- `DataCriacao` - Data de cadastro

### Tabela: Candles
- `Id` - Identificador único
- `AtivoId` - FK para Ativos
- `Data` - Data e hora do candle
- `Abertura` - Preço de abertura
- `Maxima` - Preço máximo
- `Minima` - Preço mínimo
- `Fechamento` - Preço de fechamento
- `ContadorCandles` - Contador sequencial

## 🔐 Autenticação (Próxima Fase)

A autenticação JWT com roles Admin e Assinante será implementada na próxima fase.
Por enquanto, o sistema está totalmente aberto para testes.

## 📱 Menu do Sistema

### Ativo
- ✅ Criar - Funcionando
- ✅ Listar - Funcionando

### Backtest
- 🔄 Criar - Em desenvolvimento
- 🔄 Listar - Em desenvolvimento

## 🐛 Solução de Problemas

### Porta já em uso
Se alguma porta estiver em uso, você pode alterar no `docker-compose.yml`:
- Frontend: linha `- "3001:3000"`
- Backend: linha `- "5001:5000"`
- Postgres: linha `- "5432:5432"`

### Container não inicia
```bash
# Ver logs detalhados
docker-compose logs [service-name]

# Rebuild completo
docker-compose down
docker-compose up --build -d
```

### Erro de conexão com banco
Verifique se o container do postgres está healthy:
```bash
docker-compose ps
```

## 🎨 Tecnologias Utilizadas

- **Backend**: .NET 8, Dapper, Npgsql, CsvHelper
- **Frontend**: Nuxt 3, Vue 3, PrimeVue, TypeScript
- **Banco**: PostgreSQL 15
- **Infra**: Docker, Docker Compose

## 📚 Próximos Passos

1. Implementar autenticação JWT
2. Implementar CRUD de Backtests
3. Adicionar gráficos e visualizações
4. Implementar lógica de estratégias
5. Relatórios e estatísticas

---

**Desenvolvido para Sistema de Backtest CPGR** 🚀

