using API.Models;
using API.Models.Interfaces;
using API.Models.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<AppDbContext>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
        }
    }
}
