using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Workflows.Models.DataEvents;
using Workflows.StorageAdapters.Definitions;

namespace Definitions.Transports
{
    public class GrpcStorageAdapter : StorageAdapter.StorageAdapterBase
    {
        private IStorageAdapter implementation;
        private readonly ILogger<GrpcStorageAdapter> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="implementation"></param>
        public GrpcStorageAdapter(IStorageAdapter implementation, ILogger<GrpcStorageAdapter> logger, IConfiguration configuration)
        {
            this.implementation = implementation;
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<PushDataReply> PushData(PushDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Received request to push data. Forwarding to implementation. {request}");
            var result = await this.implementation.PushDataToStorage(request.SourceFilePath);
            
            if (_configuration["DeleteDataAfterUse"] == "true")
            {
                // Delete the file that has just been copied from the output to the permanent storage.
                File.Delete(request.SourceFilePath);
                
                // Delete the file that triggered the step from the input storage.
                File.Delete(request.DeletePath);
            }
            
            var reply = new PushDataReply
            {
                GeneratedMetadata = result
            };
            
            return reply;
        }

        public override async Task<PullDataReply> PullData(PullDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Received request to pull data: {request}");
            await this.implementation.PullDataFromStorage(request.Metadata, request.TargetPath);
            return new PullDataReply
            {
                IsSuccess = true
            };
        }
    }
}