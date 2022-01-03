using api.Models.Entities;
using api.Models.Entities.Auth;
using api.Models.Entities.Chat;
using api.Models.Entities.HouseMap;
using api.Models.Entities.Other;
using api.Models.Entities.Rooms;
using api.Models.Entities.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //configure relation

            builder.Entity<ContactForm>();

            //table names
            builder.Entity<Location>().ToTable("Locations");

            // 
            builder.Entity<Image>()
                .ToTable("Images")
                .HasDiscriminator<int>("ImageType")
                .HasValue<HouseImage>(1)
                .HasValue<RoomImage>(2);

            builder.Entity<Comment>()
                .ToTable("Comments")
                .HasDiscriminator<int>("CommentType")
                .HasValue<HouseComment>(1)
                .HasValue<RoomComment>(2);

            //one to one
            builder.Entity<House>()
                .ToTable("Houses")
                .HasOne(h => h.Location)
                .WithOne(l => l.House)
                .HasForeignKey<Location>(l => l.HouseId);

            // one to many
            builder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation);
            builder.Entity<Conversation>()
                .HasMany(c => c.Participants)
                .WithOne(p => p.Conversation);

            builder.Entity<House>()
                .HasOne(h => h.User)
                .WithMany(u => u.Houses)
                .HasForeignKey(h => h.UserId);

            builder.Entity<Room>()
                .ToTable("Rooms")
                .HasOne(r => r.User)
                .WithMany(u => u.Rooms)
                .HasForeignKey(r => r.UserId);
            builder.Entity<House>()
                .HasMany(r => r.Rooms)
                .WithOne(h => h.House);
              
        }
    }
}
