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

app.MapGet("/books", async (BooksDbContext db) => await db.Books.ToListAsync());

app.MapGet("/books/{id}", async (int id, BooksDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    return book is not null ? Results.Ok(book) : Results.NotFound();
}
).WithName("GetBookById");

app.MapPost("/books", async (NewBookDTO newBook, BooksDbContext db) =>
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
    return Results.Created($"/books/{book.Id}", book);
}).WithName("CreateBook");

app.MapPut("books/{id}", async (int id, NewBookDTO updatedBook, BooksDbContext db) =>
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
    return Results.StatusCode(201);
}).WithName("UpdateBook");

app.MapDelete("/books/{id}", async (int id, BooksDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();
    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
