namespace MyApp.Tests;

using System.IO.Compression;
using MyApp;

public class ProgramTest {
    string path;
    public ProgramTest() {
        if(!Directory.Exists("../../../../AuctionSystem-replication")){
            ZipFile.ExtractToDirectory("../../../../AuctionSystem-replication.zip", "../../../../");
        }
        path = "../../../../AuctionSystem-replication";
    }

    [Fact]
    public void CommitAuthorMode()
    {
        var expected = new Dictionary<String, Dictionary<String, int>>() {
            {"Draosakel", new ()},
            {"Christoffer H. Nielsen", new () {
                {"23-11-2022",2},
                {"28-11-2022",3}
            }},
            {"DanielNygaard00", new ()},
            {"mbjnitu", new () {
                {"28-11-2022",1}
            }},
            {"mbjn", new (){
                {"23-11-2022",3},        
                {"28-11-2022",3}
            }},
            {"Patrick Matthiesen", new (){
                {"14-11-2022",1},        
                {"09-11-2022",1}
            }},
            {"Patrick", new () {
                {"21-10-2022",3}
            }}
        };

        var actual = Program.CommitAuthorMode(path);

        actual.Should().BeEquivalentTo(expected);
    }

}