using api.Models.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.Rooms
{
    public class RoomComment : Comment
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }

        public RoomComment(string content, Guid userId, int roomId) : base(content, userId) 
        {
            RoomId = roomId;
        }
        public RoomComment() : base() { }
    }
}
