using WebApi.Application.Repositories.Interfaces;
using WebApi.Infastructure.Data;
using WebApi.Infastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


// add DbContext
builder.Services.AddDbContext<ApplicationDbContext>();


// Add services to the container.
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
