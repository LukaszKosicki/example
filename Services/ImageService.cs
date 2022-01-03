using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public class ImageService
    {
        public static Dictionary<string, int> sizes = new Dictionary<string, int>() { { "small", 600 }, { "medium", 900 }, { "large", 1920 } };
        public static void ResizeImage(string inputPath)
        { 
            foreach (var size in sizes)
            {
                using (Image image = Image.Load(inputPath))
                {
                    if (image.Height > size.Value && image.Width > size.Value)
                    {
                        int width = 0;
                        int height = 0;
                        if (image.Width >= image.Height)
                        {
                            width = size.Value;
                            height = (int)(image.Height * (Convert.ToDecimal(size.Value) / image.Width));
                        } else
                        {
                            height = size.Value;
                            width = (int)(image.Width * (Convert.ToDecimal(size.Value) / image.Height));
                        }
                        image.Mutate(i => i.Resize(width, height));
                    }
                    image.Save(inputPath.Replace("original", size.Key));
                }            
            }
        }

        public static void MoveImages(string beginningPath, string fileName, string extension, string directoryPath, string oldPath)
        {
            Directory.CreateDirectory(directoryPath);
            foreach (var size in sizes)
            {
                string fullFileName = fileName + "." + size.Key + extension;
                File.Move(Path.Combine(oldPath, fullFileName), Path.Combine(directoryPath, fullFileName));
            }
            string originalFileName = fileName + "." + "original" + extension;
            File.Move(Path.Combine(oldPath, originalFileName), Path.Combine(directoryPath, originalFileName));
        }

        public static void DeleteImages(string beginningPath, string fileName, string extension)
        {
            foreach (var size in sizes)
            {
                string fullFileName = fileName + "." + size.Key + extension;
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", beginningPath, fullFileName ));
            }
            string originalFileName = fileName + "." + "original" + extension;
            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", beginningPath, originalFileName));
        }
    }
}
