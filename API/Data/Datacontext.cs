using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Datacontext : IdentityDbContext<AppUsers, AppRole, int, 
        IdentityUserClaim<int>,AppUserRole, IdentityUserLogin<int> , 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public Datacontext(DbContextOptions options) : base(options){
        }
        /*public DbSet<AppUsers>? Users { get; set; }*/
        public DbSet<UserLike>? Likes { get; set; }
        public DbSet<Message>? Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder){
            base.OnModelCreating(builder);

            builder.Entity<AppUsers>()
             .HasMany(ur => ur.UserRoles)
             .WithOne(u => u.User)
             .HasForeignKey(u => u.UserId)
             .IsRequired();

            builder.Entity<AppRole>()
             .HasMany(ur => ur.UserRoles)
             .WithOne(u => u.Role)
             .HasForeignKey(u => u.RoleId)
             .IsRequired();

            builder.Entity<UserLike>()
             .HasKey(k => new { k.SourceUserID, k.LikedUserID});

            builder.Entity<UserLike>()
              .HasOne(s => s.SourceUser)
              .WithMany(s => s.LikedUsers)
              .HasForeignKey(s => s.SourceUserID)
              .OnDelete(DeleteBehavior.Cascade);

             builder.Entity<UserLike>()
              .HasOne(s => s.LikedUser)
              .WithMany(s => s.LikedByUsers)
              .HasForeignKey(s => s.LikedUserID)
              .OnDelete(DeleteBehavior.Cascade);

             builder.Entity<Message>()
             .HasOne(u => u.Recipient)
             .WithMany(u => u.MessageReceveid)
             .OnDelete(DeleteBehavior.Restrict);

             builder.Entity<Message>()
             .HasOne(u => u.Sender)
             .WithMany(u => u.MessageSent)
             .OnDelete(DeleteBehavior.Restrict);
             
        }
        
    }
}