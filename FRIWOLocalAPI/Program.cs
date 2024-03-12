using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using FRIWOLocalAPI.Models;
using FRIWOLocalAPI.SerialPorts;
using Microsoft.AspNetCore.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:5002", "http://localhost:5000", "http://fvn-s-ws01.friwo.local:85");
        });
});
builder.Services.AddSingleton<SerialService>();
builder.Services.AddDbContext<SerialPortContext>(opt =>
    opt.UseInMemoryDatabase("SerialPortList"));
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "FRIWOLocalAPI", Version = "v1" });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FRIWOLocalAPI v1"));
}

//app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
