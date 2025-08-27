using WebApi.Application.Repositories.Interfaces;
using WebApi.Application.Services;
using WebApi.Application.Services.Interfaces;
using WebApi.Infastructure.Data;
using WebApi.Infastructure.Repositories;
using WebApi.Infrastructure.Repositories;
using WebApi.Infrastructure.Services;
using WebApi.Infrastructure.Services.Interfaces;
using WebApi.Web.Extensions;
using WebApi.Web.Identity;

var builder = WebApplication.CreateBuilder(args);


// add DbContext
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddCustomIdentity();
builder.Services.AddCustomAuthentication(builder.Configuration);


// Add repositories
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();

// Add services to the container.
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtSwaggerGen();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
