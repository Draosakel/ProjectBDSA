using LibGit2Sharp;

namespace MyApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppContext();

            var path = Console.ReadLine();
            if (path == "" || path == null) {
                path = @"C:\Users\Ejer\source\repos\ProjectBDSA";
            }
            Console.WriteLine(path);
            SaveCommitsToDB(path, db);
            db.SaveChanges();

            PrintCommitAuthorMode(CommitsAuthorToIterableDictionary(db));
            Console.WriteLine("----------------------------------------------------------------");
            PrintCommitFrequencyMode(CommitsFrequencyToIterableDictionary(db));
        }

        public static void CommitAuthorModeToDB(Dictionary<String, Author> cam, AppContext db){
            foreach(KeyValuePair<String, Author> entry in cam)
            {
                db.Add(entry.Value);
            } 
            db.SaveChanges();            
        }

        public static void PrintCommitAuthorMode(Dictionary<String, Dictionary<String, int>> cam) {
            foreach(KeyValuePair<String, Dictionary<String, int>> entry in cam)
            {
                Console.WriteLine(entry.Key);
                entry.Value
                    .Select(pair => new String("  " + pair.Value + " " + pair.Key))
                    .ToList()
                    .ForEach(pair => Console.WriteLine(pair));
                Console.WriteLine("");
            }   
        }

        public static void PrintCommitFrequencyMode(Dictionary<String, int> commits) {
            foreach(KeyValuePair<String, int> entry in commits)
            {
                Console.WriteLine(entry.Value + " " + entry.Key);
            }  
        }

        public static Dictionary<String, int> CommitsFrequencyToIterableDictionary(AppContext db) {
            var authorCommits = db.Authors.ToList();
            var commitDict = new Dictionary<String, int>();
            foreach (var author in authorCommits)
            foreach (var commit in author.Commits) {
                if(commitDict.ContainsKey(commit.Date)) {
                commitDict[commit.Date] += 1;
                } else
                commitDict.Add(commit.Date, 1);
            }
            return commitDict;
        }

        public static Dictionary<String, Dictionary<String, int>> CommitsAuthorToIterableDictionary(AppContext db) {
            var authorCommits = db.Authors.ToList();
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

        public static void SaveCommitsToDB(String path, AppContext db){
            if(Repository.IsValid(path))
            {
                using (var repo = new Repository(path))
                {
                    var commits = repo.Branches.SelectMany(x => x.Commits)
                        .GroupBy(x => x.Sha)
                        .Select(x => x.First())
                        .ToArray();
                    foreach (var commit in commits)
                    {
                        if (db.Commits.Find(commit.ToString()) != null) {
                            continue;
                        }

                        var commitAuthor = commit.Author.Name;

                        var dbAuthor = db.Authors.Find(commitAuthor);

                        if (dbAuthor != null) {
                            dbAuthor.Commits.Add(
                                new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")}
                            );
                        }
                        else {
                            db.Authors.Add(
                                new Author() {AuthorId = commitAuthor, Commits = new List<Commit>()
                                    { new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")} }
                                }
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
}