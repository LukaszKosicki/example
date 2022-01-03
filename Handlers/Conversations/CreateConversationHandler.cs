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
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Handlers.Conversations
{
    public class CreateConversationHandler : IRequestHandler<CreateConversationCommand, IdentityResult>
    {
        protected readonly ApplicationDbContext AppDbContext;
        private readonly IHubContext<MemberHub> _hubContext;
        private ActiveMembersStore _activeMembers;
        public CreateConversationHandler(ApplicationDbContext appDbContext, IHubContext<MemberHub> hubContext, ActiveMembersStore activeMembers)
        {

            this.AppDbContext = appDbContext;
            this._hubContext = hubContext;
            this._activeMembers = activeMembers;
        }
        public async Task<IdentityResult> Handle(CreateConversationCommand model, CancellationToken cancellationToken)
        {
            var member = await AppDbContext.Members
                .Include(m => m.ConverstionMemberships)
                .ThenInclude(m => m.Conversation)
                .ThenInclude(m => m.Members)
                .FirstOrDefaultAsync(m => m.MemberId == model.MemberId);
            var reciever = await AppDbContext.Members
                .Include(m => m.ConverstionMemberships)
                .ThenInclude(m => m.Conversation)
                .ThenInclude(m => m.Members)
                .FirstOrDefaultAsync(m => m.MemberId == model.ReciverId);

            if (member == null || reciever == null || member.Banned)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "user_not_exist", Description = "Użytkownik nie istnieje lub został zablokowany" });
            }

            var membersConversation = member.ConverstionMemberships
                .FirstOrDefault(m =>
                    m.Conversation.Members
                    .FirstOrDefault(c => c.MemberId == model.ReciverId) != null);

            Conversation conversation = new Conversation();
            var msgType = "ReceiveConversation";
            if (membersConversation != null)
            {
                conversation = membersConversation.Conversation;
                msgType = "ReceiveMessage";
            }
            var memberConv = member.AddConversation(conversation, reciever, model.Message);
            var recieverConv = reciever.AddConversation(conversation, member, model.Message);

            var message = new Message(conversation, member.MemberId, model.Message, member.UserName);
            conversation.AddMessage(message);
            memberConv.SetAsReaded();
            //recieverConv.SetAsReaded();
            await AppDbContext.SaveChangesAsync();

            var conversationMembers = new int[] { member.MemberId, reciever.MemberId };
            var dtos = new LoggedInConversationMemberDto[]
            {
                Mapper.Map<LoggedInConversationMemberDto>(memberConv),
                Mapper.Map<LoggedInConversationMemberDto>(recieverConv)
            };

            if (membersConversation != null)
            {
                var messageDto = Mapper.Map<MessageDto>(message);
                var userInfoReciever = _activeMembers.GetUsersInfo(member.MemberId);
                await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                    .SendAsync("ReceiveConversation", dtos[0]);

                foreach (var memberId in conversationMembers)
                {
                    userInfoReciever = _activeMembers.GetUsersInfo(memberId);
                    await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                        .SendAsync("ReceiveMessage", messageDto);
                }
                return IdentityResult.Success;
            }

            for (int i = 0; i < conversationMembers.Length; i++)
            {
                var userInfoReciever = _activeMembers.GetUsersInfo(conversationMembers[i]);
                await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                    .SendAsync("ReceiveConversation", dtos[i]);
            }

            return IdentityResult.Success;
        }

    }
}

/*
            
            var conversation = member.AddConversation(reciever, model.Message);
            conversation.AddMember(reciever);
            //reciever.AddToConversation(conversation);
            
            await AppDbContext.SaveChangesAsync();

            var message = new Message(conversation,member.MemberId,model.Message,member.Identity.UserName);
            conversation.AddMessage(message);
            
            await AppDbContext.SaveChangesAsync();

            var messageDto = Mapper.Map<MessageDto>(message);
            var conversationMembers = new int[]{member.MemberId, member.MemberId};

            foreach (var memberId in conversationMembers){
                var userInfoReciever = _activeMembers.GetUsersInfo(memberId);
                await _hubContext.Clients.Clients(userInfoReciever.Select(u => u.ConnectionId).ToList())
                    .SendAsync("ReceiveNewConversation", messageDto);
            } */