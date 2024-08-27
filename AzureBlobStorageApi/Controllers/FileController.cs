using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobStorageApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly string _connecionString;
        private readonly string _containerName;

        public FileController(IConfiguration configuration)
        {
            _connecionString = configuration.GetValue<string>("BlobConnecionString");
            _containerName = configuration.GetValue<string>("BlobContainerName");
        }

        [HttpPost("upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            var container = new BlobContainerClient(_connecionString, _containerName);
            var blob = container.GetBlobClient(file.FileName);

            using var data = file.OpenReadStream();
            blob.Upload(data, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
            });

            return Ok(blob.Uri.ToString());
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var container = new BlobContainerClient(_connecionString, _containerName);
            var blob = container.GetBlobClient(fileName);

            if (!blob.Exists())
                return BadRequest();

            var result = blob.DownloadContent();

            return File(result.Value.Content.ToArray(), result.Value.Details.ContentType, blob.Name);
        }

        [HttpDelete("delete/{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            var container = new BlobContainerClient(_connecionString, _containerName);
            var blob = container.GetBlobClient(fileName);

            blob.DeleteIfExists();

            return NoContent();
        }

        [HttpGet("list")]
        public IActionResult ListFiles()
        {
            var blobsDTO = new List<BlobDTO>();
            var container = new BlobContainerClient(_connecionString, _containerName);

            foreach (var blob in container.GetBlobs())
                blobsDTO.Add(
                    new BlobDTO(
                        blob.Name,
                        blob.Properties.ContentType,
                        container.Uri.AbsoluteUri + "/" + blob.Name
                    )
                );

            return Ok(blobsDTO);
        }
    }
}