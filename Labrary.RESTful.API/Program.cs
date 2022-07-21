

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
builder.Services.AddTransient<IBookService, BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// EndPoints

app.MapGet("api/books", async (IBookService _service) => {
    var dtos = await _service.GetAll();
    return dtos is not null ? Results.Ok(dtos) : Results.StatusCode(404);
}).WithDisplayName("GetBooks");

app.Run();