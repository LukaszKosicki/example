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
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Handlers.Conversations
{
    public class RemoveConversationHandler : IRequestHandler<RemoveConversationCommand, IdentityResult>
    {
        protected readonly ApplicationDbContext AppDbContext;
        private readonly IHubContext<MemberHub> _hubContext;
        private ActiveMembersStore _activeMembers;
        public RemoveConversationHandler(ApplicationDbContext appDbContext, IHubContext<MemberHub> hubContext, ActiveMembersStore activeMembers)
        {

            this.AppDbContext = appDbContext;
            this._hubContext = hubContext;
            this._activeMembers = activeMembers;
        }
        public async Task<IdentityResult> Handle(RemoveConversationCommand model, CancellationToken cancellationToken)
        {
            var member = await AppDbContext.Members
                .Include(m => m.ConverstionMemberships)
                .FirstOrDefaultAsync(m => m.MemberId == model.MemberId);

            var membersConversation = member.ConverstionMemberships.FirstOrDefault(c => c.ConversationId == model.ConversationId);

            if (member == null || member.Banned)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "user_not_exist", Description = "Użytkownik nie istnieje lub został zablokowany" });
            }
            if (membersConversation == null)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "conversation_not_exist", Description = "Konwersacja nie istnieje" });
            }
            var conversation = await AppDbContext.Conversations
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == model.ConversationId);

            membersConversation.SetAsHidden();
            await AppDbContext.SaveChangesAsync();

            var userInfoReciever = _activeMembers.GetUsersInfo(model.MemberId);
            await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                .SendAsync("ConversationRemoved", conversation.Id);

            return IdentityResult.Success;
        }
    }
}