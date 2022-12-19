using LibGit2Sharp;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var repo = new Repository(@"C:\Users\Ejer\source\repos\ProjectBDSA")) {
                Console.WriteLine(repo.Commits.Count());
            }
        }
    }
}