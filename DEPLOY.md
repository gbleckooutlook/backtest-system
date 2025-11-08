# 🚀 Deploy - Desenvolvimento vs Produção

## 📋 Visão Geral

Este projeto suporta dois modos:
- **Desenvolvimento (localhost)**: Para desenvolvimento local
- **Produção (DuckDNS)**: Para acesso externo via domínio público

---

## 🏠 Modo Desenvolvimento (Padrão)

### Uso:
```bash
docker-compose up -d
```

### Acesso:
- **Frontend**: http://localhost:5173
- **Backend**: http://localhost:5001
- **Postgres**: localhost:5432

### CORS Permitido:
- `http://localhost:3000`
- `http://localhost:3001`
- `http://localhost:5173`

---

## 🌐 Modo Produção (DuckDNS)

### Uso:
```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
```

### Acesso:
- **Frontend**: http://leandrocpgr.duckdns.org:5173
- **Backend**: http://leandrocpgr.duckdns.org:5001

### CORS Permitido:
- Todos os de desenvolvimento +
- `http://leandrocpgr.duckdns.org:5173`
- `https://leandrocpgr.duckdns.org:5173`
- `http://leandrocpgr.duckdns.org`
- `https://leandrocpgr.duckdns.org`

### Configuração:

O arquivo `docker-compose.prod.yml` sobrescreve:

```yaml
backend:
  environment:
    - PUBLIC_DOMAIN=leandrocpgr.duckdns.org
    - FRONTEND_PORT=5173

frontend:
  environment:
    - NUXT_PUBLIC_API_BASE_URL=http://leandrocpgr.duckdns.org:5001
```

---

## 🔧 Personalizar Domínio

### Editar `docker-compose.prod.yml`:
```yaml
services:
  backend:
    environment:
      - PUBLIC_DOMAIN=seu-dominio.duckdns.org  # ← Altere aqui
      
  frontend:
    environment:
      - NUXT_PUBLIC_API_BASE_URL=http://seu-dominio.duckdns.org:5001  # ← Altere aqui
```

---

## 🐛 Debug CORS

Para ver quais origens estão permitidas, verifique os logs do backend:

```bash
docker logs backtest-backend
```

---

## 📝 Comandos Úteis

### Desenvolvimento:
```bash
# Subir
docker-compose up -d

# Rebuild
docker-compose up -d --build

# Ver logs
docker-compose logs -f
```

### Produção:
```bash
# Subir
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Rebuild
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build

# Ver logs
docker-compose -f docker-compose.yml -f docker-compose.prod.yml logs -f
```

### Parar tudo:
```bash
docker-compose down
```

---

## 🔐 Configurar Firewall

Para acesso externo, libere as portas no roteador:

| Serviço  | Porta Externa | Porta Interna | Protocolo |
|----------|---------------|---------------|-----------|
| Frontend | 5173          | 5173          | TCP       |
| Backend  | 5001          | 5001          | TCP       |

---

## ✅ Checklist de Deploy

- [ ] DuckDNS configurado e funcionando
- [ ] Portas 5173 e 5001 liberadas no roteador (port forwarding)
- [ ] `docker-compose.prod.yml` com domínio correto
- [ ] Rebuild dos containers: `docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build`
- [ ] Testar acesso externo: http://seu-dominio.duckdns.org:5173
- [ ] Verificar logs sem erros de CORS

---

## 🆘 Troubleshooting

### Erro: "CORS policy blocked"
- ✅ Verifique se o domínio está correto em `docker-compose.prod.yml`
- ✅ Rebuild do backend: `docker-compose up -d --build backend`

### Erro: "ERR_CONNECTION_REFUSED"
- ✅ Verifique se as portas estão liberadas no firewall
- ✅ Verifique se o DuckDNS está apontando para o IP correto
- ✅ Teste localmente primeiro: `curl http://localhost:5001/api/ativos`

### Frontend não conecta no backend
- ✅ Verifique `NUXT_PUBLIC_API_BASE_URL` em `docker-compose.prod.yml`
- ✅ Deve usar o domínio público, não `localhost`

