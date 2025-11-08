# 🦆 DuckDNS Auto-Updater

## 📋 Descrição

Serviço em background que atualiza automaticamente o IP no DuckDNS a cada 30 minutos (configurável).

---

## ⚙️ Configuração

### Desenvolvimento (Desabilitado por padrão)

No `appsettings.json`:
```json
{
  "DuckDns": {
    "Enabled": false,
    "Token": "",
    "Domain": "",
    "IntervaloMinutos": 30
  }
}
```

### Produção (Habilitado)

No `appsettings.Production.json`:
```json
{
  "DuckDns": {
    "Enabled": true,
    "Token": "15555d67-4714-4283-9dac-b33d867dc564",
    "Domain": "leandrocpgr",
    "IntervaloMinutos": 30
  }
}
```

---

## 🚀 Como Funciona

1. **Startup**: Atualiza o IP imediatamente ao iniciar
2. **Loop**: Atualiza a cada X minutos (padrão: 30)
3. **Logs**: Registra todas as atualizações

### Exemplo de Log:
```
[DuckDNS] Serviço iniciado. Atualizando a cada 30 minutos.
[DuckDNS] Atualizando IP para domínio: leandrocpgr
[DuckDNS] ✅ IP atualizado com sucesso em 08/11/2025 15:30:00
```

---

## 🔧 Personalizar Intervalo

### Atualizar a cada 15 minutos:
```json
{
  "DuckDns": {
    "IntervaloMinutos": 15
  }
}
```

### Atualizar a cada 1 hora:
```json
{
  "DuckDns": {
    "IntervaloMinutos": 60
  }
}
```

---

## 🛡️ Segurança

⚠️ **IMPORTANTE**: Não commite o `appsettings.Production.json` com o token real!

### Adicionar ao `.gitignore`:
```
appsettings.Production.json
```

### Usar variáveis de ambiente (recomendado):
```bash
export DuckDns__Token="seu-token-aqui"
export DuckDns__Domain="seu-dominio"
export DuckDns__Enabled="true"
```

---

## 🐛 Debug

### Ver logs em tempo real:
```bash
docker logs -f backtest-backend | grep DuckDNS
```

### Desabilitar temporariamente:
```json
{
  "DuckDns": {
    "Enabled": false
  }
}
```

---

## ✅ Checklist

- [ ] Configurar Token no `appsettings.Production.json`
- [ ] Configurar Domain
- [ ] Definir `Enabled: true`
- [ ] Rebuild do backend: `docker-compose up -d --build backend`
- [ ] Verificar logs: `docker logs backtest-backend`
- [ ] Confirmar sucesso: procurar por "✅ IP atualizado"

---

## 🆘 Troubleshooting

### Erro: "Token ou Domain não configurado"
✅ Verifique se o `appsettings.Production.json` está correto

### Erro: "Erro de rede ao atualizar IP"
✅ Verifique sua conexão com a internet
✅ Teste manualmente: https://www.duckdns.org/update?domains=leandrocpgr&token=SEU_TOKEN&ip=

### Serviço não está executando
✅ Verifique se `Enabled: true`
✅ Verifique se está em modo Production: `ASPNETCORE_ENVIRONMENT=Production`

---

## 📖 Referência

- **DuckDNS API**: https://www.duckdns.org/spec.jsp
- **Resposta de Sucesso**: `OK`
- **Resposta de Erro**: `KO` (bad auth, bad domain, etc)

---

## 💡 Dicas

1. **IP Dinâmico**: Recomendado manter intervalo de 30 minutos
2. **IP Fixo**: Pode aumentar para 60+ minutos
3. **Logs**: Sempre verifique os logs após mudanças

