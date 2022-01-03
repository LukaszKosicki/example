using api.Models.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.Rooms
{
    public class RoomImage : Image
    {
        public RoomImage(string begginingPath, string fileName, string extension) : base(begginingPath, fileName, extension) { }
        public RoomImage(string begginingPath, string fileName, string extension, int roomId) : base(begginingPath, fileName, extension)
        {
            RoomId = roomId;
        }
        public RoomImage() : base() {}
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
