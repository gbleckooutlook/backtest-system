-- Criação das tabelas do sistema de backtest

-- Tabela de Ativos
CREATE TABLE IF NOT EXISTS Ativos (
    Id SERIAL PRIMARY KEY,
    Nome VARCHAR(255) NOT NULL,
    Mercado VARCHAR(50) NOT NULL,
    Codigo VARCHAR(50) NOT NULL,
    Timeframe VARCHAR(50) NOT NULL,
    NomeArquivoCsv VARCHAR(255),
    DataCriacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Candles
CREATE TABLE IF NOT EXISTS Candles (
    Id SERIAL PRIMARY KEY,
    AtivoId INTEGER NOT NULL,
    Data TIMESTAMP NOT NULL,
    Abertura DECIMAL(18, 5) NOT NULL,
    Maxima DECIMAL(18, 5) NOT NULL,
    Minima DECIMAL(18, 5) NOT NULL,
    Fechamento DECIMAL(18, 5) NOT NULL,
    ContadorCandles INTEGER NOT NULL,
    CONSTRAINT FK_Candles_Ativos FOREIGN KEY (AtivoId) REFERENCES Ativos(Id) ON DELETE CASCADE
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS IX_Candles_AtivoId ON Candles(AtivoId);
CREATE INDEX IF NOT EXISTS IX_Candles_Data ON Candles(Data);


