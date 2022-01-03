using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos.Conversation;
using API.Queries.Conversations;
using AutoMapper;
using Data.Contexts;
using Domain.Entities.Conversations;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Handlers.Conversations.Queries
{
    public class GetConversationHandler : IRequestHandler<GetConversationQuery, ConversationDto>
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;

        public GetConversationHandler(ApplicationDbContext appDbContext, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            this._appDbContext = appDbContext;
            this._userManager = userManager;
            this._mediator = mediator;
        }

        public async Task<ConversationDto> Handle(GetConversationQuery model, CancellationToken cancellationToken)
        {

            var conversation = await _appDbContext.Conversations
                .AsNoTracking()
                .Include(m => m.Messages) //.TakeLast(2)
                .Include(m => m.Members)
                .FirstOrDefaultAsync(m => m.Id == model.ConversationId);
            var convMember = conversation.Members.FirstOrDefault(m => m.MemberId == model.MemberId);
            if (conversation == null || convMember == null)
            {
                return null;
            }
            var messages = conversation.Messages.Where(n => n.SendDate > convMember.LastlyReaded && n.SendDate > convMember.DisplayData)
                .Union(conversation.Messages.Where(n => n.SendDate < convMember.LastlyReaded && n.SendDate > convMember.DisplayData).OrderByDescending(n => n.SendDate).Take(25));
            var conv = Mapper.Map<ConversationDto>(conversation);
            conv.Messages = Mapper.Map<List<MessageDto>>(messages);
            return conv;
        }
    }
}