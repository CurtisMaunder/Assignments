using AdminWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using AdminWebAPI.Models.DataManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaWebContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(McbaWebContext)));

    //Enable lazy loading
    //options.UseLazyLoadingProxies();
});

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<BillPayManager>();

builder.Services.AddControllers()
    .AddNewtonsoftJson(options => {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
