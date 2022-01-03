using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using api.Models.ViewModels.Files;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadFiles([FromForm] UploadFilesViewModel model)
        {
            string folderName = String.IsNullOrEmpty(model.FolderName) ? Guid.NewGuid().ToString() : model.FolderName; 

            string pathFragment = Path.Combine(model.CategoryName, folderName);
            List<FileModel> result = await FileService.SaveFiles(model.Files, pathFragment);

            return Ok(new { folderName, result });
        }

        [HttpDelete]
        [Authorize]
        public IActionResult DeleteFile([FromBody] DeleteFileViewModel model)
        {
            if (model.FileType == "image")
            {
                ImageService.DeleteImages(model.BeginningPath, model.FileName, model.Extension);
            }

            return Ok();
        }
    }
}