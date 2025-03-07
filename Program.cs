using CosmosApp.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<CosmosClient>(
    new CosmosClient(
        accountEndpoint: builder.Configuration["CosmosDb:AccountEndpoint"],
        authKeyOrResourceToken: builder.Configuration["CosmosDb:AccountKey"]
));
builder.Services.AddScoped<CosmosSetupService>();
builder.Services.AddScoped<CosmosCandidateService>();
builder.Services.AddScoped<CosmosCandidateNotesService>();
builder.Services.AddScoped<CosmosCandidateNoteArchivesService>();

builder.Services.AddHostedService<CandidateNoteProcessorService>();

builder.Services.AddControllers();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapControllers();

//app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
