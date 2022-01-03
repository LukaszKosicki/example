using api.Models.Entities.Auth;
using api.Models.Entities.Shared;
using api.Models.ViewModels.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.Rooms
{
    public enum RoomTypes
    {
        Bathroom,
        Bedroom,
        DiningRoom,
        Kitchen,
        LivingRoom,
        Garage,
        Hall,
        Study,
        Attic,
        Cellar,
        Porch,
        Larder,
        Mezzanine,
        UtilityRoom,
        WalkInWardrobe
    }

    public class Room : BaseEntity
    {
        public Room() { }
        public Room(string dsc, int type, float roomArea)
        {
            Description = dsc;
            RoomType = (RoomTypes)type;
            RoomArea = roomArea;
        }
        public float RoomArea { get; set; }
        public string Description { get; set; }
        public RoomTypes RoomType { get; set; }
        public List<RoomImage> Images { get; set; }
        public List<RoomComment> Comments { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public House House { get; set; }

        public void UpdateRoom(UpdateRoomViewModel model, House house)
        {
            Description = model.Description;
            RoomType = (RoomTypes)model.RoomType;
            RoomArea = model.RoomArea;
            House = house;
        }
    }
}
