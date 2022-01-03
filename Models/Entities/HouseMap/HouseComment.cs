using api.Models.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.HouseMap
{
    public class HouseComment : Comment
    {
        public int HouseId { get; set; }
        public House House { get; set; }

        public HouseComment(string content, Guid userId, int houseId) : base(content, userId) 
        {
            HouseId = houseId;
        }
        public HouseComment() : base() { }
    }
}
