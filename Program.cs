
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using YoutubeStreamBot;

// var grids = new List<GridInfo>();"http://144.126.255.72:4444/wd/hub"

var grids = new List<GridInfo>
{
    new GridInfo
    {
        Url = "http://144.126.255.72:4444/wd/hub",
        Browser = "edge",
        Platform = "LINUX",
        Limit = 25,
        Used = 0
    },
    new GridInfo
    {
        Url = "http://35.229.141.143:4444/wd/hub",
        Browser = "chrome",
        Platform = "LINUX",
        Limit = 14,
        Used = 0
    },
};


var noBots = 20;

// pp.GetZippedPugin("proxy2.zip");

var bots = new List<YoutubeAutomation>();
int gridIndex = 0;

var pp = new ProxyPlugin("all.dc.smartproxy.com", 10001, "user-sp52cbgyij", "1234567");

// Create bots
for (int i = 0; i < noBots; i++)
{
    var grid = grids[gridIndex];
    grid.Used++;
    if (grid.Used >= grid.Limit)
    {
        gridIndex++;
    }

    bots.Add(new YoutubeAutomation(pp, grid, "파워사다리", "https://youtu.be/3bimA8nakDI", true, i));
}

// Run bots
foreach (var bot in bots)
{
    Task.Run(() =>
    {
        try
        {
            bot.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    });
}

// Stop bots
Console.WriteLine("Press any key to stop bots...");
Console.ReadKey();
foreach (var bot in bots)
{
    bot.Stop();
}

// exit program
Environment.Exit(0);
