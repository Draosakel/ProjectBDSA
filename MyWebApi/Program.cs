using MyWebApi;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:5258",
                                              "http://localhost:5053")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
                                              ;
                      });
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/downloadrepos/{user}/{repo}", async (string user, string repo) =>
    {
        using var db = new AppContext();
        return Cloner.CloneRepo(user, repo, db);
    }
);


app.Run();