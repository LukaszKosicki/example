using API.Models.Entity.Auth;
using API.Models.Entity.Categories;
using API.Models.Entity.Common;
using API.Models.Entity.Offers;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasData(
                    new Category { Id = 1, Name = "Wszystkie", Color = "#00ffff", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = null },
                    new Category { Id = 2, Name = "Budowlane", Color = "#fff8dc", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = 1 },
                    new Category { Id = 3, Name = "Budowa", Color = "#e9967a", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = 2 },
                    new Category { Id = 4, Name = "Remont", Color = "#fffaf0", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = 2 },
                    new Category { Id = 5, Name = "IT", Color = "#e6e6fa", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = 1 },
                    new Category { Id = 6, Name = "Programowanie", Icon = "fa-list", Color = "#fffacd", CreationDate = DateTime.Now, ParentCategoryId = 5 },
                    new Category { Id = 7, Name = "Administracja", Icon = "fa-list", Color = "#ffb6c1", CreationDate = DateTime.Now, ParentCategoryId = 5 }
                );

      


        /*    modelBuilder.Entity<Location>()
                .HasData(
                new Location
                {
                    Id = 1,
                    City = "Poznań",
                    Street = "Czarnkowska",
                    Number = "2"
                });

            modelBuilder.Entity<Offer>()
                .HasData(
                    new Offer {
                        Id = 1,
                        Title = "Aplikacja internetowa", 
                        Description = "Zlecę wykonanie aplikacji",
                        CreationDate = DateTime.Now, 
                        Budget = (decimal)9999.99,
                        LocationId = 1,
                        Categories =
                        {
                             new Category { Id = 1, Name = "Wszystkie", Color = "#00ffff", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = null },
                              new Category { Id = 5, Name = "IT", Color = "#e6e6fa", Icon = "fa-list", CreationDate = DateTime.Now, ParentCategoryId = 1 },
                    new Category { Id = 6, Name = "Programowanie", Icon = "fa-list", Color = "#fffacd", CreationDate = DateTime.Now, ParentCategoryId = 5 }
                        }
                    }
                );*/

        }
    }
}
