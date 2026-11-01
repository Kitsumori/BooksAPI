using BooksApi.DB;
using BooksApi.DTO;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<BooksDbContext>(options =>
    options.UseInMemoryDatabase("BooksDatabase"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var books = app.MapGroup("/books");

books.MapGet("/", GetAllBooks);

books.MapGet("/{id}", GetBookById);

books.MapPost("/", CreateBook);

books.MapPut("/{id}", UpdateBook);

books.MapDelete("/{id}", DeleteBook);

app.Run();

static async Task<IResult> GetAllBooks(BooksDbContext db) => TypedResults.Ok(await db.Books.ToListAsync());

static async Task<IResult> GetBookById(int id, BooksDbContext db)
{
    var book = await db.Books.FindAsync(id);
    return book is not null ? TypedResults.Ok(book) : TypedResults.NotFound();
}

static async Task<IResult> CreateBook(NewBookDTO newBook, BooksDbContext db)
{
    var lastId = db.Books.Any() ? db.Books.Max(b => b.Id) + 1 : 1;
    var book = new BookDTO
    {
        Id = lastId,
        Title = newBook.Title,
        Author = newBook.Author,
        YearPublished = newBook.YearPublished
    };

    db.Books.Add(book);
    await db.SaveChangesAsync();
    return TypedResults.Created($"/books/{book.Id}", book);
}

static async Task<IResult> UpdateBook(int id, NewBookDTO updatedBook, BooksDbContext db)
{
    var book = await db.Books.FindAsync(id);
    if (book is null)
    {
        return Results.NotFound();
    }

    book.Title = updatedBook.Title;
    book.Author = updatedBook.Author;
    book.YearPublished = updatedBook.YearPublished;

    await db.SaveChangesAsync();
    return TypedResults.StatusCode(201);
}

static async Task<IResult> DeleteBook(int id, BooksDbContext db)
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();
    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}