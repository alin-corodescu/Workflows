using System.Collections.Generic;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestGrpcService.Definitions;
using Workflows.Models;

namespace TestGrpcService.Clients
{
    public class ComputeStepGrpcClient : IComputeStep
    {
        private readonly ILogger<ComputeStepGrpcClient> _logger;
        private readonly ComputeStepService.ComputeStepServiceClient _client;

        public ComputeStepGrpcClient(ILogger<ComputeStepGrpcClient> logger, IConfiguration configuration)
        {
            _logger = logger;
            var computeAddr = configuration["Sidecar:ComputeAddr"];
            logger.LogInformation($"Connecting to {computeAddr}");
            using var channel = GrpcChannel.ForAddress(computeAddr);
            this._client = new ComputeStepService.ComputeStepServiceClient(channel);
        }

        public IAsyncEnumerable<ComputeStepReply> TriggerCompute(ComputeStepRequest request)
        {
            return this._client.TriggerCompute(request).ResponseStream.ReadAllAsync();
        }
    }
}