using api.Models.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entities.Shared
{
    public enum CommentTypes {
        House,
        Room
    }
    public abstract class Comment : BaseEntity
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Comment() { }

        public Comment(string content, Guid userId)
        {
            Content = content;
            UserId = userId;
        }
    }
}
