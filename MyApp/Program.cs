using LibGit2Sharp;
using System.IO.Compression;

namespace MyApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(!Directory.Exists("../../../../AuctionSystem-replication")){
                ZipFile.ExtractToDirectory("../../../../AuctionSystem-replication.zip", "../../../../");
            }
            var path = "../../../../AuctionSystem-replication";
            using var db = new AppContext();
            //CommitAuthorModeToDB(CommitAuthorMode(path), db);

            PrintCommitAuthorMode(CommitsAuthorToIterableDictionary(db));
            Console.WriteLine("----------------------------------------------------------------");
            PrintCommitFrequencyMode(CommitsFrequencyToIterableDictionary(db));

            //Delete unzipped folder
            Directory.Delete("../../../../../../../AuctionSystem-replication", true);
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
            foreach (var author in authorCommits) {
                if(author.Commits == null) continue;
                foreach (var commit in author.Commits) {
                    if(commitDict.ContainsKey(commit.Date)) {
                    commitDict[commit.Date] += 1;
                    } else
                    commitDict.Add(commit.Date, 1);
                }
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

        public static Dictionary<String, Author> CommitAuthorMode(String path){
            if(Repository.IsValid(path))
            {
                var commitDict = new Dictionary<String, Author>();
                using (var repo = new Repository(path))
                {
                    var commits = repo.Branches.SelectMany(x => x.Commits)
                        .GroupBy(x => x.Sha)
                        .Select(x => x.First())
                        .ToArray();
                    foreach (var commit in commits)
                    {
                        var commitName = commit;
                        var commitDate = commit.Author.When;
                        var commitDateFormat = commit.Author.When.Date.ToString().Replace(" 00:00:00", "");
                        var commitAuthor = commit.Author.Name;
                        if(commitDict.ContainsKey(commitAuthor)) {
                            commitDict[commitAuthor].Commits.Add(new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")});
                        }
                        else
                        commitDict.Add(commitAuthor, new Author() {AuthorId = commitAuthor, Commits = new List<Commit>() {new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")}}});
                    }
                }
                return commitDict;
            }
            throw new ArgumentException();
        }
    }
}