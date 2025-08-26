using WebApi.Application.Repositories.Interfaces;
using WebApi.Infastructure.Data;
using WebApi.Infastructure.Repositories;
using WebApi.Web.Extensions;
using WebApi.Web.Identity;

var builder = WebApplication.CreateBuilder(args);


// add DbContext
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddCustomIdentity();
builder.Services.AddCustomAuthentication(builder.Configuration);


// Add services to the container.
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
