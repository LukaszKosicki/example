using api.Hubs;
using api.Hubs.Clients;
using api.Models.Entities.Auth;
using api.Models.Entities.Chat;
using api.Models.Interfaces;
using api.Models.ViewModels.Chat;
using api.Models.ViewModels.Hubs.Chat;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ConversationController : BaseController
    {
        private IAsyncRepository<Conversation> conversationRepository;
        private IAsyncRepository<Participant> participantRepository;
        private readonly UserManager<User> userManager;
        private readonly IHubContext<ChatHub, IChatClient> _chatHub;
        public ConversationController(IMapper mapper, IAsyncRepository<Conversation> convRepo, 
            IAsyncRepository<Participant> partRepo,
            UserManager<User> userMgr,
            IHubContext<ChatHub, IChatClient> chatHub) : base(mapper)
        {
            conversationRepository = convRepo;
            participantRepository = partRepo;
            userManager = userMgr;
            _chatHub = chatHub;
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation(CreateConversationViewModel model)
        {
            Conversation conversation = await conversationRepository.Entities.Include(c => c.Participants)
                .SingleOrDefaultAsync(c => c.Participants.SingleOrDefault(p => p.UserId == Guid.Parse(model.SenderId)) != null
                && c.Participants.SingleOrDefault(p => p.UserId == Guid.Parse(model.RecipientId)) != null); 

            if (conversation == null)
            {
                conversation = new Conversation(new List<Participant>()
                {
                    new Participant(Guid.Parse(model.SenderId)),
                    new Participant(Guid.Parse(model.RecipientId))
                });

                await conversationRepository.Add(conversation);
            }

            return Ok(conversation.Id);
        }

        [HttpGet]
        public async Task<IActionResult> GetConversations(string userId, string recipientUserName)
        {
            var participants = await participantRepository.Entities.Include(p => p.User)
                .Include(p => p.Conversation)
                .ThenInclude(p => p.Participants)
                .ThenInclude(p => p.User)
                .Include(p => p.Conversation)
                .ThenInclude(c => c.LastMessage)
                .ThenInclude(lm => lm.User)
                .Where(p => p.UserId.ToString() == userId)
                .OrderByDescending(p => p.Conversation.LastMessage.CreatedDate)
                .ToListAsync();

            var result = _mapper.Map<List<GetConversationViewModel>>(participants);

            if (recipientUserName != null)
            {
                var conversation = result.FirstOrDefault(p => p.ParticipantName == recipientUserName);
                if (conversation != null)
                {
                    result.Remove(conversation);
                    result.Insert(0, conversation);
                }       
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            var conversation = await conversationRepository.Entities.Include(c => c.Participants).ThenInclude(p => p.User)
                .SingleOrDefaultAsync(c => c.Id == model.ConversationId);
            conversation.AddMessage(new Message(model.Content, Guid.Parse(model.UserId)));

            await conversationRepository.Update(conversation);

            foreach(var participant in conversation.Participants)
            {
                await _chatHub.Clients.Clients(ChatHub.ConnectionIds.GetConnections(participant.User.UserName).ToList())
                    .ReceiveMessage(_mapper.Map<HubMessageViewModel>(model));
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages(int conversationId)
        {
            var conversation = await conversationRepository.Entities.Include(c => c.Messages)
                .SingleOrDefaultAsync(c => c.Id == conversationId);

            return Ok(_mapper.Map<List<GetMessageViewModel>>(conversation.Messages));
        }
    }
}
