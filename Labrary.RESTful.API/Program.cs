using Labrary.RESTful.API.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// DbContext
builder.Services.AddDbContext<DataContext>(opts =>
       opts.UseSqlServer(connectionString:builder.Configuration.GetConnectionString("defaultconnection")));
// Automapper
builder.Services.AddAutoMapper(typeof(Program));

// Custom Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddTransient<ISaveFiles, SaveLocalFile>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// EndPoints
app.MapGet("api/books", async (IBookService _service) => {
    var dtos = await _service.GetAll();
    return dtos is not null ? Results.Ok(dtos) : Results.StatusCode(404);
}).WithDisplayName("GetBooks");

app.MapGet("api/books/{Id:int}", async (IBookService _service, int Id) =>
{
    var dtos = await _service.GetById(Id);
    return dtos is not null ? Results.Ok(dtos): Results.NotFound("There is nothing here.");
}).WithName("GetBook");



app.MapPost("api/books", async (IBookService _service,[FromForm] BookCreateDto model)=>
{

    await _service.Create(model);
    return Results.Created("GetBook", model);
});



app.Run();