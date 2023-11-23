using System;

namespace YoutubeStreamBot
{
    public class RemoteTesting
    {
        public static void Test()
        {

            var grids = new List<GridInfo>
            {
                new GridInfo
                {
                    Url = "http://24.199.76.56:4444/wd/hub",
                    Browser = "edge",
                    Platform = "LINUX",
                    Limit = 25,
                    Used = 0
                },
                new GridInfo
                {
                    Url = "http://24.199.70.210:4444/wd/hub",
                    Browser = "edge",
                    Platform = "LINUX",
                    Limit = 25,
                    Used = 0
                },
            };


            var noBots = 1;

            // pp.GetZippedPugin("proxy2.zip");

            var bots = new List<YoutubeAutomation>();
            int gridIndex = 0;

            var pp = new ProxyPlugin("kr.smartproxy.com", 10001, "user-sp52cbgyij", "1234567");

            // Create bots
            for (int i = 0; i < noBots; i++)
            {
                var grid = grids[gridIndex];
                grid.Used++;
                if (grid.Used >= grid.Limit)
                {
                    gridIndex++;
                }

                bots.Add(new YoutubeAutomation(pp, grid, "파워사다리", "https://youtu.be/BX-0Lff02OM", true, i));
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
        }
    }
}
