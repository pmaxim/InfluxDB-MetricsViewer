using InfluxDB.Client.Core.Flux.Domain;
using MetaMetrics.Api;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetaMetricsViewer.Service
{
    public class InfluxDBService
    {
        private readonly MetaMetricsClientService _clientService;
        private readonly InfluxDBOptions _options;

        public InfluxDBService(IOptions<InfluxDBOptions> options, MetaMetricsClientService clientService)
        {
            _clientService = clientService;
            _options = options.Value;
        }


        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(100);

        public async Task<IEnumerable<FluxTable>> QueryAsync(string query)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();
                using var influxClient = await _clientService.Create();
                var api = influxClient.GetQueryApi();
                var tables = await api.QueryAsync(query, _options.Org);
                return tables.ToArray();
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}