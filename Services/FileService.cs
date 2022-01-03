using api.Models.ViewModels.Files;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public static class FileService
    {
        public static async Task<List<FileModel>> SaveFiles(IList<IFormFile> files, string pathFragment)
        {
            List<FileModel> result = new List<FileModel>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string newFileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(formFile.FileName);

                    result.Add(new FileModel
                    {
                        BeginningPath = pathFragment,
                        FileName = newFileName,
                        Extension = extension
                    });

                    newFileName = formFile.ContentType.Contains("image") ? newFileName + ".original" + extension :
                        newFileName + extension;

                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", pathFragment);
                    Directory.CreateDirectory(path);
                    path = Path.Combine(path, newFileName);

                    using (var stream = File.Create(path))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    // jesli zdjecie to zmien rozmiar
                    if (formFile.ContentType.Contains("image"))
                    {
                        ImageService.ResizeImage(path);
                    }
                    
                }
            }
            return result;
        }
    }
}
