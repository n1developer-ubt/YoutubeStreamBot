using System;

namespace YoutubeStreamBot
{
    public class LocalTest
    {
        public static void Test()
        {
            List<string> profiles = new List<string>(){
                "Profile 3",
                "Profile 4",
                "Profile 5",
                "Profile 6",
                "Profile 7",
                "Profile 8",
                "Profile 9",
                "Profile 10",
                "Profile 11",
                "Profile 12",
                "Profile 13",

            };

            var pp = new ProxyPlugin("kr.smartproxy.com", 10001, "user-sp52cbgyij", "1234567");

            var listOfBots = new List<YoutubeAutomation>();

            int index = 0;
            foreach (var profile in profiles)
            {
                var bot = new YoutubeAutomation(pp, null, null, null, false, index++, profile);
                listOfBots.Add(bot);
                bot.Run();
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            foreach (var bot in listOfBots)
            {
                bot.Stop();
            }
        }
    }
}
