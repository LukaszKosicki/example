using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Queries.Conversations;
using Data.Contexts;
using Domain.Entities.Conversations;
using Domain.Entities.Users;
using Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Handlers.Conversations.Queries
{
    public class GetConversationsHandler : IRequestHandler<GetConversationsQuery, IEnumerable<ConversationMember>>
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;

        public GetConversationsHandler(ApplicationDbContext appDbContext, UserManager<ApplicationUser> userManager, IMediator mediator)
        {
            this._appDbContext = appDbContext;
            this._userManager = userManager;
            this._mediator = mediator;
        }

        public async Task<IEnumerable<ConversationMember>> Handle(GetConversationsQuery model, CancellationToken cancellationToken)
        {

            var member = await _appDbContext.Members
                .AsNoTracking()
                .Include(m => m.ConverstionMemberships)
                .ThenInclude(c => c.Conversation)
                .ThenInclude(c => c.Members)
                .ThenInclude(m => m.Member)
                .FirstOrDefaultAsync(m => m.MemberId == model.MemberId);

            if (member == null)
            {
                return null;
            }
            var conversations = member.ConverstionMemberships.Where(c => !c.Conversation.Deleted &&
                c.Conversation.Members.FirstOrDefault(m => m.MemberId == member.MemberId).DisplayData < c.Conversation.LastSendDate);

            return conversations;
        }
    }
}