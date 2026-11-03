using BDPYMES.LogicaServicio.Servicios;


var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("BddPyme")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'BddPyme' en appsettings.json.");

builder.Services.AddSingleton(new PymesService());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("PermitirFrontend");
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();
