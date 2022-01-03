using api.Models.Entities.Auth;
using api.Models.Entities.Shared;
using System;

namespace api.Models.Entities.Chat
{
    public class Participant : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime MessagesReadAt { get; set; }
        public Conversation Conversation { get; set; }
        public int ConversationId { get; set; }
        public Participant(Guid userId)
        {
            UserId = userId;
        }
    }
}
