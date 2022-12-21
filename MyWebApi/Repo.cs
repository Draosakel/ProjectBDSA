using LibGit2Sharp;

namespace MyWebApi;

public static class Repo {
    public static void CloneRepo() {
        var repo = Repository.Clone("https://github.com/Draosakel/Assignment-0", "../repos/");
    }
}