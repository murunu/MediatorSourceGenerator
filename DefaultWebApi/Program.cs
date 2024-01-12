using Mediator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediator();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", async (IMediator mediator) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    await mediator.SendAsync("bruh");

    stopwatch.Stop();

    return $"Time: {stopwatch.ElapsedMilliseconds}ms";
});

app.Run();