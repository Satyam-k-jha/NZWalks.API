using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IamgesController : ControllerBase
    {

        public IamgesController(IImageRepository imageRepository)
        {
            ImageRepository = imageRepository;
        }

        public IImageRepository ImageRepository { get; }

        //POST: /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageRequestUploadDto request)
        {
            ValidateFileUpload(request);
            if (ModelState.IsValid)
            {
                //Convert Dtor to Domain model 
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName).ToLower(),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription
                };

                //User repository to upload image
                await ImageRepository.Upload(imageDomainModel);
                return Ok(imageDomainModel);
            }
            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageRequestUploadDto request)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName).ToLower()))
            {
                ModelState.AddModelError("File", "Only .jpg, .jpeg, and .png files are allowed.");
            }
            if (request.File.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("File", "File size should not exceed 10MB.");
            }
        }
    }
}
