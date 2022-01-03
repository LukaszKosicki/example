using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.Shared
{
    public abstract class Image : BaseEntity
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string BeginningPath { get; set; }

        public Image() { }

        public Image(string begginingPath, string fileName, string extension)
        {
            BeginningPath = begginingPath;
            FileName = fileName;
            Extension = extension;
        }
    }
}
