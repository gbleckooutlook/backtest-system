# 🚀 Primeira Execução - Guia Rápido

## Para seu amigo que vai rodar pela primeira vez

Olá! Este é um guia simplificado para você rodar o sistema pela primeira vez.

### ⚡ Início Rápido (3 passos)

1. **Instale o Docker Desktop**
   - Windows/Mac: https://www.docker.com/products/docker-desktop
   - Aguarde o Docker iniciar completamente (ícone verde)

2. **Execute o script de inicialização**
   - **Windows**: Clique duas vezes no arquivo `start.bat`
   - **Mac/Linux**: Abra o terminal na pasta do projeto e execute `./start.sh`

3. **Acesse o sistema**
   - Abra o navegador em: http://localhost:3001
   - Pronto! O banco de dados já foi criado automaticamente 🎉

### 📊 Testando o Sistema

1. Clique em "Criar Ativo" no menu
2. Preencha os dados:
   - Nome: Mini-Dólar
   - Mercado: B3
   - Código: WDO
   - Timeframe: 5 minutos
3. Faça upload de um arquivo CSV (se tiver)
4. Clique em "Salvar"

### 📝 Formato do CSV

Se for fazer upload de dados históricos, o CSV deve ter este formato:

```csv
Data, Abertura, Máxima, Mínima, Fechamento, Contador de Candles
2025-01-01 09:00:00,5000.00,5050.00,4990.00,5025.00,1
2025-01-01 09:05:00,5025.00,5060.00,5020.00,5055.00,2
```

### ❌ Se algo der errado

1. **Docker não inicia**
   - Reinicie o computador
   - Abra o Docker Desktop manualmente

2. **Porta já em uso**
   - Feche outros programas que possam estar usando as portas 3001 ou 5001
   - Ou execute: `docker-compose down` e tente novamente

3. **Página não carrega**
   - Aguarde 1-2 minutos (primeira vez demora mais)
   - Verifique se os 3 containers estão rodando: `docker ps`
   - Deve aparecer: backtest-frontend, backtest-backend, backtest-postgres

4. **Erro ao criar ativo**
   - Verifique os logs: `docker-compose logs backend`
   - Execute o reset do banco:
     ```bash
     docker-compose down
     docker volume rm backtest-system_postgres_data
     docker-compose up -d
     ```

### 🛑 Para Parar o Sistema

- Execute: `docker-compose stop`
- Ou feche o Docker Desktop

### 🔄 Para Rodar Novamente (dias seguintes)

Basta executar o `start.bat` novamente! Os dados estarão salvos.

### 📖 Documentação Completa

- [README.md](README.md) - Documentação completa
- [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Soluções para problemas

### 💡 Dicas

- ✅ Os dados ficam salvos mesmo depois de parar os containers
- ✅ Não precisa recriar o banco toda vez
- ✅ Para resetar tudo: `docker-compose down -v` e suba novamente
- ✅ O sistema funciona offline (não precisa de internet depois de baixar)

### 🎯 URLs Importantes

- **Frontend**: http://localhost:3001
- **Backend API**: http://localhost:5001
- **Documentação API**: http://localhost:5001/swagger (se habilitado)

---

**Dúvidas?** Consulte o arquivo [TROUBLESHOOTING.md](TROUBLESHOOTING.md) ou o [README.md](README.md)

