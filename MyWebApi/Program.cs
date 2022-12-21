using Microsoft.EntityFrameworkCore;
using LibGit2Sharp;
using MyWebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/downloadrepos/{user}/{repo}", async (string user, string repo, TodoDb db) =>
    {
        Repo.CloneRepo();
        return "123";
    }
);


app.Run();