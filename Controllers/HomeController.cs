using api.Models.Entities;
using api.Models.Entities.Rooms;
using api.Models.Interfaces;
using api.Models.ViewModels.Home;
using api.Models.ViewModels.House;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        private IAsyncRepository<House> houseRepository;
        private IAsyncRepository<Room> roomRepository;
        private readonly IMapper _mapper;

        public HomeController(IAsyncRepository<House> repo, IMapper mapper, IAsyncRepository<Room> roomRepo)
        {
            houseRepository = repo;
            roomRepository = roomRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetHouses()
        {
            var houses = houseRepository.Entities
                .Include(h => h.Images)
                .Include(h => h.User)
                .Include(h => h.Location)
                .Take(10).OrderByDescending(h => h.CreatedDate);

            return Ok(_mapper.Map<List<GetHousesViewModel>>(houses));
        }

        [HttpGet]
        public IActionResult GetRooms()
        {
            var rooms = roomRepository.Entities
                .Include(r => r.User)
                .Include(r => r.Images)
                .Take(10).OrderByDescending(r => r.CreatedDate);

            return Ok(_mapper.Map<List<GetRoomsViewModel>>(rooms));
        }
    }
}
