using API.Models.Entity.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Extensions
{
    public static class ConversationExtensions
    {
        public static ICollection<Conversation> FindConversationByUserId(this ICollection<Conversation> conversations, string userId)
        {
            return conversations.Where(c => c.ConversationMembers.FirstOrDefault(cm => cm.UserId == Guid.Parse(userId)) != null).ToList(); 
        }
    }
}
