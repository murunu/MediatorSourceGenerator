using Mediator;
using NativeAOTWebApi.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediator();

var app = builder.Build();

app.MapAsyncEndpoints();
app.MapVoidEndpoints();

app.Run();

// Add this for testing purposes
public partial class Program { }