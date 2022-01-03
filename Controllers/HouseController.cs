using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entities;
using api.Models.Entities.Auth;
using api.Models.Entities.HouseMap;
using api.Models.Entities.Rooms;
using api.Models.Entities.Shared;
using api.Models.Interfaces;
using api.Models.ViewModels.House;
using api.Models.ViewModels.Users;
using api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HouseController : Controller
    {
        private readonly string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "houses", "temp");
        private IAsyncRepository<House> houseRepository;
        private IAsyncRepository<Room> roomRepository;
        private readonly UserManager<User> userManager;
        private readonly IMapper _mapper;
        private readonly string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "houses");

        public HouseController(IAsyncRepository<House> repo, IAsyncRepository<Room> roomRepo, IMapper mapper, UserManager<User> userMgr)
        {
            houseRepository = repo;
            _mapper = mapper;
            userManager = userMgr;
            roomRepository = roomRepo;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddHouse([FromBody] AddHouseViewModel model)
        {
            // dodawanie house i location do bazy danych 
            House house = new House(model.ProjectName, model.Description, model.LandArea, model.ChangesInProject, model.ConstructionStage);
            house.AddLocation(model.Location);
            house.UserId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            await houseRepository.Add(house);

            // tworzenie List<Image> i dodanie do house
            List<HouseImage> images = new List<HouseImage>();
            foreach (var image in model.Images)
            {
                string newDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "houses", house.Id.ToString());
                ImageService.MoveImages(image.BeginningPath, image.FileName, image.Extension, newDirectoryPath, oldPath);
                images.Add(new HouseImage(Path.Combine("images", "houses", house.Id.ToString()), image.FileName, image.Extension));
            }
            house.AddImages(images);
            await houseRepository.Update(house);

            //usuwanie plikow
            foreach (var image in model.DeleteImages)
            {
                ImageService.DeleteImages(image.BeginningPath, image.FileName, image.Extension);
            }
       
            return Ok(house.Id);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateHouse([FromBody] UpdateHouseViewModel model)
        {
            var house = await houseRepository.Entities
                .Include(h => h.Location)
                .Include(h => h.Images)
                .SingleOrDefaultAsync(h => h.Id == model.Id);

            if (house == null)
            {
                return NotFound();
            }
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (house.UserId != user.Id)
            {
                return Problem(statusCode: 500);
            }

            foreach (var image in model.DeleteImages)
            {
                var entity = house.Images.SingleOrDefault(i => i.FileName == image.FileName);
                if (entity != null)
                {
                    house.Images.Remove(entity);
                }
                ImageService.DeleteImages(image.BeginningPath, image.FileName, image.Extension);
            }

            foreach (var image in model.Images)
            {
                if (image.BeginningPath.Contains("temp"))
                {
                    string newDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "houses", house.Id.ToString());
                    ImageService.MoveImages(image.BeginningPath, image.FileName, image.Extension, newDirectoryPath, oldPath);
                    house.Images.Add(new HouseImage(Path.Combine("images", "houses", house.Id.ToString()), image.FileName, image.Extension));
                }
            }

            house.UpdateHouse(model);

            await houseRepository.Update(house);
            return Ok(house.Id);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveHouse(int id, bool withRooms)
        {
            var house = await houseRepository.Entities.Include(r => r.Rooms).SingleOrDefaultAsync(h => h.Id == id);
            var user = await userManager.GetUserAsync(HttpContext.User);
            
            if (house == null)
            {
                return NotFound();
            }

            if (user.Id != house.UserId)
            {
                return Problem("Obiekt nie należy do Ciebie!", "", 500, "", "");
            }
          
            foreach(var room in house.Rooms)
            {
                await roomRepository.Remove(room);
            }
            

            await houseRepository.Remove(house);
            var pathToImages = Path.Combine(path, id.ToString());
            if (Directory.Exists(pathToImages))
            {
                Directory.Delete(pathToImages, true);
            }
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetHouses(string projectName, string voivodeship, int constructionStage)
        {
            var houses = await houseRepository.Entities.Include(h => h.Location).Include(h => h.Images).Include(h => h.User).ToListAsync();

            if (projectName != null) houses = houses.Where(h => h.ProjectName.ToLower().Contains(projectName.ToLower())).ToList();
            if (voivodeship != null && voivodeship != "Wszystkie Województwa") houses = houses.Where(h => h.Location.Voivodeship == voivodeship).ToList();
            if (constructionStage != -1) houses = houses.Where(h => h.ConstructionStage == (ConstructionStages)constructionStage).ToList();

            return Ok(_mapper.Map<List<HousesListViewModel>>(houses));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHouseById(int id)
        {
            var house = await houseRepository.Entities
                .Include(h => h.Images)
                .Include(h => h.Location)
                .Include(h => h.User)
                .SingleOrDefaultAsync(i => i.Id == id);
   
            return Ok(_mapper.Map<GetHouseViewModel>(house));
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetHousesByUserId(string userId)
        {
            var houses = await houseRepository.Entities.Include(h => h.Images).Include(h => h.User)
                .Where(h => h.UserId == Guid.Parse(userId)).ToListAsync();

            return Ok(_mapper.Map<List<UserHousesListViewModel>>(houses));

        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetHousesListByUserName(string userName)
        {
            var houses = await houseRepository.Entities
                .Where(h => h.User.UserName == userName).ToListAsync();

            return Ok(_mapper.Map<List<ListHouseNamesViewModel>>(houses));

        }

        [HttpGet("{userName}")]
        public async Task<IActionResult> GetHousesByUserName(string userName)
        {
            var houses = await houseRepository.Entities.Include(h => h.Images).Include(h => h.User)
                .Where(h => h.User.UserName == userName).ToListAsync();

            return Ok(_mapper.Map<List<UserHousesListViewModel>>(houses));

        }
    }
}