using Microsoft.AspNetCore.Mvc;
using Mod.FilesCoverotor.Server.Covertors;

namespace Mod.FilesCovertor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConvertController : ControllerBase
    {
        private static readonly string[] ConvertToFiles = new[]
        {
            ".png",".pdf"
        };

        private readonly ILogger<ConvertController> _logger;

        public ConvertController(ILogger<ConvertController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string result = "hello world";

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ConvertRequest convertRequest)
        {
            string result = "";
            // Assuming MyRequestModel is a simple class with a property 'Input'
            if (convertRequest == null || string.IsNullOrWhiteSpace(convertRequest.ContentAs64))
            {
                return BadRequest("Input cannot be empty");
            }
            if (!ConvertToFiles.Contains(convertRequest.ToExtension))
            {
                return BadRequest($"can not convert to: {convertRequest.ToExtension}");
            }
            ConvertManager convertManager = new ConvertManager();
            string extension = Path.GetExtension(convertRequest.FileName);

            switch (extension) {
                case ".doc":
                case ".docx":
                    result = convertManager.ConvertDocxToSvg(convertRequest);
                    break;

                case ".pdf":
                    if (convertRequest.ToExtension == ".pdf")
                        result = convertRequest.ContentAs64;
                    else
                        result = convertManager.ConvertPdfToImage(convertRequest);
                    break;

                case ".ppt":
                case ".pptx":
                    if(convertRequest.ToExtension == ".pdf")
                        result = convertManager.ConvertPptToPdf(convertRequest);
                    else
                        result = convertManager.ConvertPptToPng(convertRequest);
                    break;
                default:
                    return BadRequest($"can not convert from: {extension}");
            }
            
            
            return Ok(result); 
        }
    }
}