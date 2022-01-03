using api.Models.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.Chat
{
    public class Conversation : BaseEntity
    {
        public Message LastMessage { get; set; }
        public List<Message> Messages { get; set; }
        public List<Participant> Participants { get; set; }

        public Conversation() { }
        public Conversation(List<Participant> participants)
        {
            Participants = participants;
        }

        public void AddMessage(Message message)
        {
            LastMessage = message;
            Messages = new List<Message>() { message };
        }
    }
}
