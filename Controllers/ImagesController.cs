using api.Models.Entities.HouseMap;
using api.Models.Entities.Rooms;
using api.Models.Entities.Shared;
using api.Models.Interfaces;
using api.Models.ViewModels.Files;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ImagesController : Controller
    {
        private IAsyncRepository<Image> imageRepository;
        private enum ImageTypes
        {
            House,
            Room
        }

        public ImagesController(IAsyncRepository<Image> repo)
        {
            imageRepository = repo;
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteFileViewModel model)
        {
           ImageService.DeleteImages(model.BeginningPath, model.FileName, model.Extension);
            var image = await imageRepository.GetById(model.Id);
            if (image != null)
            {
                await imageRepository.Remove(image);
            }

           return Ok();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImages([FromForm] UploadFilesViewModel model)
        {
            const string folderName = "temp";
            string pathFragment = Path.Combine(model.CategoryName, folderName);
            List <FileModel> result = await FileService.SaveFiles(model.Files, pathFragment);

            return Ok(result);
        }
    }
}
