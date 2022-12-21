using LibGit2Sharp;

namespace MyWebApi;

public static class Cloner {
    public static void CloneRepo(string user, string repo, AppContext db) {
        var repo1 = repo;
        if (System.IO.Directory.Exists("../repos/" + repo1)) {
            var count = 1;
            while (System.IO.Directory.Exists("../repos/" + repo1))
            {
                repo1 = repo + "(" + count + ")";
                count++;   
            }          
        }
        var path = "https://github.com/" + user + "/" + repo;
        Repository.Clone(path, "../repos/" + repo1);
        SaveCommitsToDB("../repos/" + repo1, db, repo1); 
    }

    public static void SaveCommitsToDB(string path, AppContext db, string repository){
        if(Repository.IsValid(path))
        {
            using (var repo = new Repository(path))
            {
                var commits = repo.Branches.SelectMany(x => x.Commits)
                    .GroupBy(x => x.Sha)
                    .Select(x => x.First())
                    .ToArray();

                db.Repos.Add(new Repo() {RepoId = repository, Authors = new List<Author>()});
                foreach (var commit in commits)
                {
                    if (db.Commits.Find(commit.ToString()) != null) {
                        continue;
                    }

                    var commitAuthor = commit.Author.Name;
                    
                    var dbAuthor =  db.Repos.Find(repository).Authors.Contains(commitAuthor);

                    if (dbAuthor) {
                        db.Repos.Find(repository).Authors.Find(commitAuthor).Commits.Add(
                            new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")}
                        );
                    }
                    else {
                        db.Repos.Find(repository).Authors.Add(
                            new Author() {AuthorId = commitAuthor, Commits = new List<Commit>()
                                { new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")}}}
                        );
                    }
                }
            }
        }
        else {
            throw new ArgumentException("Non valid path");
        }
    }
}