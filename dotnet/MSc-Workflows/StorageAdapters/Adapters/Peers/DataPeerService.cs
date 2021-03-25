using System;
using System.IO;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Workflows.Models.DataEvents;

namespace StorageAdapters.Peers
{
    /// <summary>
    /// Implementation for the data peer service
    /// </summary>
    public class DataPeerService : Workflows.Models.DataEvents.DataPeerService.DataPeerServiceBase
    {
        private readonly ILogger<DataPeerService> _logger;
        private IConfiguration _configuration;

        private string _permStorageBasePath;
        private string _addr;

        public DataPeerService(ILogger<DataPeerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _addr = configuration["NODE_IP"];
            _permStorageBasePath = configuration["StorageAdapter:PermStoragePath"];
        }


        public override async Task GetData(PeerDataRequest request, IServerStreamWriter<PeerDataReplyChunk> responseStream, ServerCallContext context)
        {
            var fileBytes = await File.ReadAllBytesAsync($"{_permStorageBasePath}/{request.Identifier.FileName}");
            
            _logger.LogInformation($"Serving the file at: {_permStorageBasePath}/{request.Identifier.FileName}");
            var peerDataReplyChunk = new PeerDataReplyChunk
            {
                Payload = ByteString.CopyFrom(fileBytes)
            };
            
            await responseStream.WriteAsync(peerDataReplyChunk);
        }
    }
}