using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Commands.Conversation;
using API.Dtos.Conversation;
using API.Dtos.LoggedInMember;
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
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, Message>
    {
        protected readonly ApplicationDbContext AppDbContext;
        private readonly IHubContext<MemberHub> _hubContext;
        private ActiveMembersStore _activeMembers;
        public SendMessageHandler(ApplicationDbContext appDbContext, IHubContext<MemberHub> hubContext, ActiveMembersStore activeMembers)
        {

            this.AppDbContext = appDbContext;
            this._hubContext = hubContext;
            this._activeMembers = activeMembers;
        }
        public async Task<Message> Handle(SendMessageCommand model, CancellationToken cancellationToken)
        {
            var member = await AppDbContext.Members
                .Include(m => m.ConverstionMemberships)
                .FirstOrDefaultAsync(m => m.MemberId == model.MemberId);

            var membersConversation = member.ConverstionMemberships.FirstOrDefault(c => c.ConversationId == model.ConversationId);

            if (member == null || member.Banned)
            {
                return null;
            }
            if (membersConversation == null)
            {
                return null;
            }
            var conversation = await AppDbContext.Conversations
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == model.ConversationId);

            var reciever = conversation.Members.FirstOrDefault(m => m.MemberId != model.MemberId);
            bool recieverEmptyConversation = reciever.DisplayData > conversation.LastSendDate;

            var message = new Message(conversation, member.MemberId, model.Message, member.UserName);
            conversation.AddMessage(message);
            membersConversation.SetAsReaded();
            await AppDbContext.SaveChangesAsync();

            var messageDto = Mapper.Map<MessageDto>(message);
            var conversationMembers = conversation.Members.Select(m => m.MemberId);

            if (recieverEmptyConversation)
            {
                var userInfoReciever = _activeMembers.GetUsersInfo(reciever.MemberId);
                await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                    .SendAsync("ReceiveConversation", Mapper.Map<LoggedInConversationMemberDto>(reciever));
            }
            foreach (var memberId in conversationMembers)
            {
                var userInfoReciever = _activeMembers.GetUsersInfo(memberId);
                await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                    .SendAsync("ReceiveMessage", messageDto);
            }

            //  await _hubContext.Clients.Clients(_activeMembers.GetUsersInfo(member.MemberId).Select(u => u.ConnectionId).ToList())
            //         .SendAsync("UpdateReadDate", membersConversation.LastlyReaded);

            return message;
        }
    }
}