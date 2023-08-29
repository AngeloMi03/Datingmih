using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
    public static class MigrationDataExtension
    {
        public static  async Task DataMigration(this IServiceProvider Services)
        {
            using var scope = Services.CreateScope();
            var service = scope.ServiceProvider;
            try
            {
                var context = service.GetRequiredService<Datacontext>();
                var userManager = service.GetRequiredService<UserManager<AppUsers>>();
                var roleManager = service.GetRequiredService<RoleManager<AppRole>>();
                await context.Database.MigrateAsync();
                await SeedData.seedData(userManager, roleManager); //params context
            }
            catch (System.Exception ex)
            {

                var logger = service.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Error occured During Migration");
            }
        }
    }
}