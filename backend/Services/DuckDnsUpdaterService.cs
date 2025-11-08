using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Services
{
    public class DuckDnsUpdaterService : BackgroundService
    {
        private readonly ILogger<DuckDnsUpdaterService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TimeSpan _intervalo;

        public DuckDnsUpdaterService(
            ILogger<DuckDnsUpdaterService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            
            // Intervalo configurável (padrão: 30 minutos)
            var minutos = _configuration.GetValue<int>("DuckDns:IntervaloMinutos", 30);
            _intervalo = TimeSpan.FromMinutes(minutos);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = _configuration.GetValue<bool>("DuckDns:Enabled", false);
            
            if (!enabled)
            {
                return;
            }

            var token = _configuration["DuckDns:Token"];
            var domain = _configuration["DuckDns:Domain"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(domain))
            {
                _logger.LogWarning("[DuckDNS] Token ou Domain não configurado. Serviço não será executado.");
                return;
            }

            // Atualizar imediatamente na primeira vez
            await AtualizarIpAsync(token, domain, stoppingToken);

            // Continuar atualizando no intervalo configurado
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_intervalo, stoppingToken);
                    await AtualizarIpAsync(token, domain, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[DuckDNS] Erro ao aguardar próxima execução.");
                }
            }
        }

        private async Task AtualizarIpAsync(string token, string domain, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"https://www.duckdns.org/update?domains={domain}&token={token}&ip=";

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.Timeout = TimeSpan.FromSeconds(30);

                var response = await httpClient.GetAsync(url, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                // Só loga se NÃO for sucesso
                if (!response.IsSuccessStatusCode || !content.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("[DuckDNS] ⚠️ Resposta inesperada: {Content}", content);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "[DuckDNS] ❌ Erro de rede ao atualizar IP.");
            }
            catch (TaskCanceledException)
            {
                // Não loga cancelamento (é esperado ao desligar)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DuckDNS] ❌ Erro ao atualizar IP.");
            }
        }
    }
}

