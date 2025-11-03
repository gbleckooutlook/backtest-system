-- Script de inicialização do banco de dados
-- Este script será executado automaticamente na primeira vez que o container subir

-- Criar tabela de Ativos
CREATE TABLE IF NOT EXISTS Ativos (
    Id SERIAL PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Mercado VARCHAR(50) NOT NULL,
    Codigo VARCHAR(20) NOT NULL,
    Timeframe VARCHAR(20) NOT NULL,
    NomeArquivoCsv VARCHAR(255),
    DataCriacao TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Criar índices para Ativos
CREATE INDEX IF NOT EXISTS idx_ativos_codigo ON Ativos(Codigo);
CREATE INDEX IF NOT EXISTS idx_ativos_mercado ON Ativos(Mercado);

-- Criar tabela de Candles
CREATE TABLE IF NOT EXISTS Candles (
    Id SERIAL PRIMARY KEY,
    AtivoId INT NOT NULL,
    Data TIMESTAMP NOT NULL,
    Abertura DECIMAL(18,2) NOT NULL,
    Maxima DECIMAL(18,2) NOT NULL,
    Minima DECIMAL(18,2) NOT NULL,
    Fechamento DECIMAL(18,2) NOT NULL,
    ContadorCandles INT NOT NULL,
    CONSTRAINT fk_candles_ativos FOREIGN KEY (AtivoId) REFERENCES Ativos(Id) ON DELETE CASCADE
);

-- Criar índices para Candles
CREATE INDEX IF NOT EXISTS idx_candles_ativoid ON Candles(AtivoId);
CREATE INDEX IF NOT EXISTS idx_candles_data ON Candles(Data);
CREATE INDEX IF NOT EXISTS idx_candles_ativoid_data ON Candles(AtivoId, Data);

-- Criar tabela de DayTrades
CREATE TABLE IF NOT EXISTS DayTrades (
    Id SERIAL PRIMARY KEY,
    DiaDayTrade DATE NOT NULL,
    AtivoId INT NOT NULL,
    DataCriacao TIMESTAMP NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_daytrades_ativos FOREIGN KEY (AtivoId) REFERENCES Ativos(Id) ON DELETE CASCADE
);

-- Criar índices para DayTrades
CREATE INDEX IF NOT EXISTS idx_daytrades_ativoid ON DayTrades(AtivoId);
CREATE INDEX IF NOT EXISTS idx_daytrades_diadaytrade ON DayTrades(DiaDayTrade);

-- Criar tabela de Trades
CREATE TABLE IF NOT EXISTS Trades (
    Id SERIAL PRIMARY KEY,
    DayTradeId INT NOT NULL,
    Gatilho1 INT NOT NULL,
    Gatilho2 INT NOT NULL,
    Regiao INT,
    Operacao VARCHAR(10) NOT NULL DEFAULT 'Compra',
    Estrategia VARCHAR(50),
    DataCriacao TIMESTAMP NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_trades_daytrades FOREIGN KEY (DayTradeId) REFERENCES DayTrades(Id) ON DELETE CASCADE
);

-- Criar índices para Trades
CREATE INDEX IF NOT EXISTS idx_trades_daytradeid ON Trades(DayTradeId);

-- Criar tabela de Backtests
CREATE TABLE IF NOT EXISTS Backtests (
    Id SERIAL PRIMARY KEY,
    DataInicio DATE NOT NULL,
    DataFim DATE NOT NULL,
    Entrada INT NOT NULL,
    Alvo INT NOT NULL,
    NumeroContratos INT NOT NULL,
    AtivoId INT NOT NULL,
    Stop INT NOT NULL,
    Folga INT NOT NULL,
    Estrategias TEXT NOT NULL,
    Proteger BOOLEAN NOT NULL DEFAULT false,
    Status VARCHAR(20) NOT NULL DEFAULT 'Iniciado',
    DataCriacao TIMESTAMP NOT NULL DEFAULT NOW(),
    DataFinalizacao TIMESTAMP,
    Resultado TEXT,
    CONSTRAINT fk_backtests_ativos FOREIGN KEY (AtivoId) REFERENCES Ativos(Id) ON DELETE CASCADE
);

-- Criar índices para Backtests
CREATE INDEX IF NOT EXISTS idx_backtests_ativoid ON Backtests(AtivoId);
CREATE INDEX IF NOT EXISTS idx_backtests_status ON Backtests(Status);
CREATE INDEX IF NOT EXISTS idx_backtests_datainicio ON Backtests(DataInicio);
CREATE INDEX IF NOT EXISTS idx_backtests_datafim ON Backtests(DataFim);

-- Log de conclusão
DO $$
BEGIN
    RAISE NOTICE 'Banco de dados inicializado com sucesso!';
    RAISE NOTICE 'Tabelas criadas: Ativos, Candles, DayTrades, Trades, Backtests';
END $$;
