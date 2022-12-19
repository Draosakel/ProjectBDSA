using LibGit2Sharp;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Console.ReadLine();
            var cam = CommitAuthorMode(path);
            var cfm = CommitFrequencyMode(path);

            PrintCommitAuthorMode(cam);
            PrintCommitFrequencyMode(cfm);
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

        public static void PrintCommitFrequencyMode(Dictionary<String, int> cfm) {
            cfm
                .Select(pair => new String(pair.Value + " " + pair.Key))
                .ToList()
                .ForEach(pair => Console.WriteLine(pair));
        }

        public static Dictionary<String, Dictionary<String, int>>  CommitAuthorMode(String path){
            if(Repository.IsValid(path))
            {
                var commitDict = new Dictionary<String, Dictionary<String, int>>();
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

                        if (commitDict.ContainsKey(commitAuthor)) {
                            if (commitDict[commitAuthor].ContainsKey(commitDateFormat)) {
                                commitDict[commitAuthor][commitDateFormat] += 1;
                            }
                            else
                            commitDict[commitAuthor].Add(commitDateFormat, 1);
                        } 
                        else 
                        commitDict.Add(commitAuthor, new Dictionary<String, int>()); 
                    }


                }
                return commitDict;
            }
            throw new ArgumentException();
        }
        public static Dictionary<String, int> CommitFrequencyMode(String path) {
            if(Repository.IsValid(path)){
                var commitDict = new Dictionary<String, int>();
                using (var repo = new Repository(path))
                    {
                    var commits = repo.Branches.SelectMany(x => x.Commits)
                        .GroupBy(x => x.Sha)
                        .Select(x => x.First())
                        .ToArray();
                    foreach (var commit in commits) {
                        var commitDate = commit.Author.When.Date.ToString().Replace(" 00:00:00", "");
                        if(commitDict.ContainsKey(commitDate)) {
                            commitDict[commitDate] += 1;
                        } else {
                            commitDict.Add(commitDate, 1);
                        }
                    }
                    return commitDict;
                }
            }
            throw new ArgumentException();
        }
    }
}