using API.Data;
using API.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using API.Extensions;
using API.Middleware;
using API.Helpers;
using API.Service;
using API.SignalR;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container..
builder.Services.AddSingleton<presenceTracker>();
builder.Services.AddScoped<ItokenService, API.Service.TokenService>();
//builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IPhotoService,PhotoService>();
//builder.Services.AddScoped<IMessageRepository,MessageRepository>();
//builder.Services.AddScoped<ILikesRepository, LikesRepositry>();
builder.Services.AddScoped<IUnitOfWorks,UnitOfWorks>();
builder.Services.AddScoped<UserActivity>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddControllers();

var config = builder.Configuration.GetSection("CloudinarySettings");
builder.Services.Configure<CloudSettings>(config);

builder.Services.AddSignalR();

String? connexionstring = builder.Configuration.GetConnectionString("DefaultConnetion");
builder.Services.AddApplicationService(connexionstring);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var _TokenKey = builder.Configuration["TokenKey"];
builder.Services.AddIdentityService(_TokenKey);

var app = builder.Build();

//await app.Services.DataMigration();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     
    app.UseSwaggerUI();
}

app.UseMiddleware<ExeptionsMiddleware>();



app.UseCors(x => x.AllowAnyHeader()
.AllowCredentials()
.WithOrigins("http://localhost:4200")
.AllowAnyMethod()
);

/*
*/

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.Run();
