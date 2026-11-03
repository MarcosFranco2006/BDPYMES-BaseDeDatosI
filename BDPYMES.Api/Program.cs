using BDPYMES.LogicaServicio.Servicios;


var builder = WebApplication.CreateBuilder(args);

// La cadena de conexión se lee de appsettings.json (sección ConnectionStrings).
string connectionString = builder.Configuration.GetConnectionString("BddPyme")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'BddPyme' en appsettings.json.");

builder.Services.AddSingleton(new PymesService(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// CORS: deja que el navegador acepte respuestas de la API cuando el front se
// abre desde otro origen (por ejemplo Live Server en el puerto 5500).
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
    // En desarrollo NO se redirige a https: esa redirección hace que el
    // navegador bloquee los fetch del front con "Failed to fetch".
    app.UseHttpsRedirection();
}

app.UseCors("PermitirFrontend");
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();
