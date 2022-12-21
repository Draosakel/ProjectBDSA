using LibGit2Sharp;
using System.Text.Json;

namespace MyWebApi;

public static class Cloner {
    public static string CloneRepo(string user, string repo, AppContext db) {
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
        db.SaveChanges();
        return CommitDictToJson(CommitsToDictionary(db, repo1));
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
                    bool dbAuthor = false;
                    
                    db.Repos.Find(repository).Authors.ForEach(
                        item => {
                            if (item.AuthorId == commitAuthor) {
                                dbAuthor = true;
                            }
                        }
                    );

                    if (dbAuthor) {
                        db.Repos.Find(repository).Authors.ForEach(
                            item => {
                                if (item.AuthorId == commitAuthor) {
                                    item.Commits.Add(
                                        new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")}
                                    );
                                }
                            }
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

    public static Dictionary<String, Dictionary<String, int>> CommitsToDictionary(AppContext db, string repository) {
        var authorCommits = db.Repos.Find(repository).Authors.ToList();
        var commitDict = new Dictionary<String, Dictionary<String, int>>();
        foreach (var authorCommit in authorCommits) {
            if (!commitDict.ContainsKey(authorCommit.AuthorId)) {
                commitDict.Add(authorCommit.AuthorId, new Dictionary<String, int>()); 
            }
            if(authorCommit.Commits == null) continue;
            foreach (var commit in authorCommit.Commits) {
                if (commitDict[authorCommit.AuthorId].ContainsKey(commit.Date)) {
                    commitDict[authorCommit.AuthorId][commit.Date] += 1;
                }
                else
                commitDict[authorCommit.AuthorId].Add(commit.Date, 1);
            }
        }
        return commitDict;
    }

    public static string CommitDictToJson(Dictionary<String, Dictionary<String, int>> cam) {
        var commitList = new List<JsonAuthors>();
        var count = 0;
        foreach(KeyValuePair<String, Dictionary<String, int>> entry in cam)
        {
            commitList.Add(new JsonAuthors() {AuthorId = entry.Key, AuthorCommits = new List<JsonCommit>()});
            foreach(KeyValuePair<String, int> entryx in entry.Value) {
                commitList[count].AuthorCommits.Add(new JsonCommit() {totalCommits = entryx.Value, commitDate = entryx.Key});
            }
            count++;
        } 
        return JsonSerializer.Serialize(commitList);
    }

    public class JsonAuthors {
        public string AuthorId { get; set; }
        public List<JsonCommit> AuthorCommits { get; set; }
    }

    public class JsonCommit {
        public int totalCommits { get; set; }
        public string commitDate { get; set; }
    }
}