using api.Models.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.HouseMap
{
    public class HouseImage : Image
    {
        public HouseImage(string begginingPath, string fileName, string extension) : base(begginingPath, fileName, extension) { }
        public HouseImage(string begginingPath, string fileName, string extension, int houseId) : base(begginingPath, fileName, extension)
        {
            HouseId = houseId;
        }
        public HouseImage() : base() { }
        public int HouseId { get; set; }
        public House House { get; set; }
    }
}
