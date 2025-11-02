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

-- Log de conclusão
DO $$
BEGIN
    RAISE NOTICE 'Banco de dados inicializado com sucesso!';
    RAISE NOTICE 'Tabelas criadas: Ativos, Candles';
END $$;

