using api.Models.Entities;
using api.Models.Entities.Auth;
using api.Models.Entities.Rooms;
using api.Models.Interfaces;
using api.Models.ViewModels.Room;
using api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RoomController : ControllerBase
    {
        private readonly string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "rooms", "temp");
        private IAsyncRepository<Room> roomRepository;
        private IAsyncRepository<House> houseRepository;
        private readonly UserManager<User> userManager;
        private readonly IMapper _mapper;
        private readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "rooms");

        public RoomController(IAsyncRepository<Room> repo, IAsyncRepository<House> houseRepo, IMapper mapper, UserManager<User> userMgr)
        {
            houseRepository = houseRepo;
            roomRepository = repo;
            _mapper = mapper;
            userManager = userMgr;
        }

        [HttpGet("{houseId}")]
        public async Task<IActionResult> GetRoomsByHouseId(int houseId)
        {
            var rooms = await roomRepository.Entities
                .Include(r => r.Images)
                .Include(r => r.House)
                .Where(r => r.House.Id == houseId).ToListAsync();
            return Ok(_mapper.Map<List<HouseRoomsViewModel>>(rooms));
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms(int roomType, int areaFrom, int areaTo)
        {
            var rooms = await roomRepository.Entities.Include(r => r.Images).Include(r => r.User).ToListAsync();
            if (roomType != -1) rooms = rooms.Where(r => r.RoomType == (RoomTypes)roomType).ToList();
            if (areaFrom > 0) rooms = rooms.Where(r => r.RoomArea >= areaFrom).ToList();
            if (areaTo > 0) rooms = rooms.Where(r => r.RoomArea <= areaTo).ToList();

            return Ok(_mapper.Map<List<GetRoomsViewModel>>(rooms));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateRoom([FromBody] UpdateRoomViewModel model)
        {
            var room = await roomRepository.Entities.Include(r => r.House).Include(r => r.Images)
                .SingleOrDefaultAsync(room => room.Id == model.Id);
            var house = room.House;
            if (room == null)
            {
                return NotFound();
            }

            if ((house == null && model.HouseId != 0) || (house != null && model.HouseId != house.Id))
            {
                house = await houseRepository.GetById(model.HouseId);
            }

            foreach (var image in model.DeleteImages)
            {
                var entity = room.Images.SingleOrDefault(i => i.FileName == image.FileName);
                if (entity != null)
                {
                    room.Images.Remove(entity);
                }
                ImageService.DeleteImages(image.BeginningPath, image.FileName, image.Extension);
            }

            foreach (var image in model.Images)
            {
                if (image.BeginningPath.Contains("temp"))
                {
                    string newDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "rooms", room.Id.ToString());
                    ImageService.MoveImages(image.BeginningPath, image.FileName, image.Extension, newDirectoryPath, oldPath);
                    room.Images.Add(new RoomImage(Path.Combine("images", "rooms", room.Id.ToString()), image.FileName, image.Extension));
                }
            }

            room.UpdateRoom(model, house);
            await roomRepository.Update(room);
            return Ok(room.Id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await roomRepository.Entities.Include(r => r.Images)
                .Include(r => r.User)
                .Include(r => r.House)
                .SingleOrDefaultAsync(i => i.Id == id);
      
            return Ok(_mapper.Map<GetRoomViewModel>(room));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddRoom([FromBody] AddRoomViewModel model)
        {
            Room room = new Room(model.Description, model.RoomType, model.RoomArea);
            room.UserId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            if (model.HouseId != 0)
            {
                room.House = await houseRepository.GetById(model.HouseId);
            }
            await roomRepository.Add(room);

            // tworzenie List<Image> i dodanie do house
            List<RoomImage> images = new List<RoomImage>();
            foreach (var image in model.Images)
            {
                string newDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "rooms", room.Id.ToString());
                ImageService.MoveImages(image.BeginningPath, image.FileName, image.Extension, newDirectoryPath, oldPath);
                images.Add(new RoomImage(Path.Combine("images", "rooms", room.Id.ToString()), image.FileName, image.Extension));
            }
            room.Images = images;
            await roomRepository.Update(room);

            //usuwanie plikow
            foreach (var image in model.DeleteImages)
            {
                ImageService.DeleteImages(image.BeginningPath, image.FileName, image.Extension);
            }

            return Ok(room.Id);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRoomsByUserId(string userId)
        {
            var rooms = await roomRepository.Entities.Include(h => h.Images).Include(h => h.User)
                .Where(h => h.UserId == Guid.Parse(userId)).ToListAsync();

            return Ok(_mapper.Map<List<GetRoomsViewModel>>(rooms));

        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetRoomsByUserName(string userName)
        {
            var rooms = await roomRepository.Entities.Include(h => h.Images).Include(h => h.User)
                .Where(h => h.User.UserName == userName).ToListAsync();

            return Ok(_mapper.Map<List<HouseRoomsViewModel>>(rooms));
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await roomRepository.GetById(id);
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user.Id != room.UserId)
            {
                return Problem("Obiekt nie należy do Ciebie!", "", 500, "", "");
            }

            if (room == null)
            {
                return NotFound();
            }

            await roomRepository.Remove(room);
            var pathToImages = Path.Combine(path, id.ToString());
            if (Directory.Exists(pathToImages))
            {
                Directory.Delete(pathToImages, true);
            }

            return Ok();
        }
    }
}
