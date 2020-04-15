using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TenantFileApi.Controllers
{
    public class ImageController : ControllerBase
    {
        private static readonly string projectId = "tenant-file-fc6de";

        private readonly ILogger<ImageController> _logger;
        private readonly StorageClient _storageClient;

        public ImageController(ILogger<ImageController> logger)
        {
            _logger = logger;
            _storageClient = StorageClient.Create();
        }

        [HttpGet("/api/images")]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var objects = await _storageClient.ListObjectsAsync("tenant-file-fc6de.appspot.com", "images/", new ListObjectsOptions { }).ToList();
            var names = objects.Select(x => x.Name);
            return Ok(names);
        }

        [HttpGet("/api/image")]
        public IActionResult GetImage([FromQuery]string name)
        {
            var stream = new MemoryStream();
            try
            {
                stream.Position = 0;
                var obj = _storageClient.GetObject("tenant-file-fc6de.appspot.com", name);
                _storageClient.DownloadObject(obj, stream);
                stream.Position = 0;

                var response = File(stream, obj.ContentType, "file.png"); // FileStreamResult
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}