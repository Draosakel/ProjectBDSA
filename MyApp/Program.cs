using LibGit2Sharp;

namespace MyApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppContext();

            var path = Console.ReadLine();
            //CommitAuthorModeToDB(CommitAuthorMode(path), db);
            //CommitFrequencyModeToDB(CommitFrequencyMode(path), db);

            //PrintCommitAuthorMode(cam);
            PrintCommitFrequencyMode(db);
        }

        public static void CommitAuthorModeToDB(Dictionary<String, Author> cam, AppContext db){
            foreach(KeyValuePair<String, Author> entry in cam)
            {
                db.Add(entry.Value);
            } 
            db.SaveChanges();            
        }

        public static void CommitFrequencyModeToDB(IEnumerable<Commit> cam, AppContext db){
            foreach(var commit in cam) {
                db.Add(commit);
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

        public static void PrintCommitFrequencyMode(AppContext db) {
            var commit = db.Commits
            .OrderBy(b => b.Date).ToList();


            foreach (var item in commit) {
                Console.WriteLine(item.Date + "  " + item.CommitId);
            }

            // if(commitDict.ContainsKey(commitDate)) {
            //                 commitDict[commitDate] += 1;
            //             } else {
            //                 commitDict.Add(commitDate, 1);
            //             }
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
                        else {
                            commitDict.Add(commitAuthor, new Author() {AuthorId = commitAuthor, Commits = new List<Commit>() {new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")}}});
                        }
                    }


                }
                return commitDict;
            }
            throw new ArgumentException();
        }
        public static IEnumerable<Commit> CommitFrequencyMode(String path) {
            if(Repository.IsValid(path)){
                using (var repo = new Repository(path))
                    {
                    var commits = repo.Branches.SelectMany(x => x.Commits)
                        .GroupBy(x => x.Sha)
                        .Select(x => x.First())
                        .ToArray();
                    foreach (var commit in commits) {
                        Console.WriteLine(commit.ToString());
                        yield return new Commit() {CommitId = commit.ToString(), Date = commit.Author.When.Date.ToString().Replace(" 00:00:00", "")};
                    }
                }
            }
            throw new ArgumentException();
        }
    }
}