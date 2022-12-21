using Microsoft.EntityFrameworkCore;
using LibGit2Sharp;
using MyWebApi;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/downloadrepos/{user}/{repo}", async (string user, string repo) =>
    {
        using var db = new AppContext();
        Cloner.CloneRepo(user, repo, db);
        return "123";
    }
);


app.Run();