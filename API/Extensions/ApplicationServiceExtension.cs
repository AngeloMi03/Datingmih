using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection Services, String? connexionstring) {
           Services.AddDbContext<Datacontext>(options => options.UseSqlite(connexionstring));
           return Services;
        }
    }
}