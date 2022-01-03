using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Commands.Conversation;
using API.Dtos.Conversation;
using API.Hubs;
using API.Hubs.Stores;
using AutoMapper;
using Data.Contexts;
using Domain.Entities.Conversations;
using Domain.Entities.Notifications;
using Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Handlers.Conversations
{
    public class MarkConversationAsReadHandler : IRequestHandler<MarkConversationAsReadCommand, ConversationMember>
    {
        protected readonly ApplicationDbContext AppDbContext;
        private readonly IHubContext<MemberHub> _hubContext;
        private ActiveMembersStore _activeMembers;
        public MarkConversationAsReadHandler(ApplicationDbContext appDbContext, IHubContext<MemberHub> hubContext, ActiveMembersStore activeMembers)
        {

            this.AppDbContext = appDbContext;
            this._hubContext = hubContext;
            this._activeMembers = activeMembers;
        }
        public async Task<ConversationMember> Handle(MarkConversationAsReadCommand model, CancellationToken cancellationToken)
        {
            var member = await AppDbContext.Members
                .Include(m => m.ConverstionMemberships)
                .FirstOrDefaultAsync(m => m.MemberId == model.MemberId);

            var membersConversation = member.ConverstionMemberships.FirstOrDefault(c => c.ConversationId == model.ConversationId);

            if (member == null)
            {
                return null;
            }
            if (membersConversation == null)
            {
                return null;
            }

            membersConversation.SetAsReaded();

            await AppDbContext.SaveChangesAsync();

            return membersConversation;
        }
    }
}