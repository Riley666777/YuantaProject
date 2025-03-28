using Microsoft.EntityFrameworkCore;
using ReadingService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NuGet.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<dbYuantaProjectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("dbYuantaProject")));

//JWT
var secretKey = "b6t8fJH2WjwYgJt7XPTqVX37WYgKs8TZ";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://yuantaprojectapi-dbg3e4e7dcb6aegr.eastasia-01.azurewebsites.net",
            ValidAudience = "https://yuantaprojectwebsite-gtcwb2bdf4hbfvdt.eastasia-01.azurewebsites.net",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("jwt_token"))
                {
                    context.Token = context.Request.Cookies["jwt_token"];
                }
                return Task.CompletedTask;
            }
        };
    });

var MyAllowSpecificOrigins = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("https://yuantaprojectwebsite-gtcwb2bdf4hbfvdt.eastasia-01.azurewebsites.net")
            .AllowCredentials() // allow cookies
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
