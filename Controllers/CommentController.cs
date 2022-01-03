using api.Models.Entities.Auth;
using api.Models.Entities.HouseMap;
using api.Models.Entities.Rooms;
using api.Models.Entities.Shared;
using api.Models.Interfaces;
using api.Models.ViewModels.Comment;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : Controller
    {
        private IAsyncRepository<Comment> commentRepository;
        private IAsyncRepository<RoomComment> roomCommentRepository;
        private IAsyncRepository<HouseComment> houseCommentRepository;
        private readonly UserManager<User> userManager;
        private readonly IMapper _mapper;

        public CommentController(IAsyncRepository<Comment> commentRepo, IAsyncRepository<RoomComment> roomComRepo,
            IAsyncRepository<HouseComment> houseComRepo, IMapper mapper, UserManager<User> userMgr)
        {
            commentRepository = commentRepo;
            _mapper = mapper;
            userManager = userMgr;
            roomCommentRepository = roomComRepo;
            houseCommentRepository = houseComRepo;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddCommentViewModel model)
        {
            var userId = Guid.Parse(userManager.GetUserId(HttpContext.User));
            if (CommentTypes.House == (CommentTypes)model.ParentType)
            {
                var entity = new HouseComment(model.Content, userId, model.ParentId);
                await commentRepository.Add(entity);
            }
            else if (CommentTypes.Room == (CommentTypes)model.ParentType)
            {
                var entity = new RoomComment(model.Content, userId, model.ParentId);
                await commentRepository.Add(entity);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int parentType, int parentId)
        {
            dynamic comments = new List<GetCommentsViewModel>();

            if (CommentTypes.House == (CommentTypes)parentType)
            {
                 comments = houseCommentRepository.Entities
                    .Include(c => c.User).Where(c => c.HouseId == parentId);
            } else if (CommentTypes.Room == (CommentTypes)parentType)
            {
                 comments = roomCommentRepository.Entities
                    .Include(c => c.User).Where(c => c.RoomId == parentId);     
            }

            return Ok(_mapper.Map<List<GetCommentsViewModel>>(comments));
        }
    }
}
