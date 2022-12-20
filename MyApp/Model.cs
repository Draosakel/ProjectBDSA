using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class AppContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Commit> Commits { get; set; }

    public string DbPath { get; }

    public AppContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "MyApp.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Author
{
    public string AuthorId { get; set; }
    public List<Commit> Commits { get; set; }
}

public class Commit
{
    public string CommitId { get; set; }
    public string Date { get; set; }
}