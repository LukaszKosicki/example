using api.Models.Entities.Auth;
using api.Models.Entities.Shared;
using System;

namespace api.Models.Entities.Chat
{
    public class Message : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public Conversation Conversation { get; set; }

        public Message(string content, Guid userId)
        {
            CreatedDate = DateTime.Now;
            Content = content;
            UserId = userId;
        }
    }
}
