using MyService.TestAPI;

//Vivek - Create builder will create an blank host builder. So lets use CreateDefaultBuilder which will give default settings givenn by Microsoft, like reading configurations from appsettings.json, reading from appsettings.dev.json when env is dev.
//var builder = WebApplication.CreateBuilder(args);

var builder = Host.CreateDefaultBuilder().ConfigureWebHostDefaults(builder =>
{
    builder.UseStartup<Startup>();
});

var app = builder.Build();

//Vivek - remove the below code, all this configurations will be handled inside the startup.cs ConfigureService() method
// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


//var app = builder.Build();

//Vivek - Below code will be run to configure every HTTP request pipeline, this will be done inside startup.Configure. Inside configure we do not have acces to app, so the extension methods are different
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseAuthorization();

//app.MapControllers();

app.Run();
