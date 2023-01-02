using System;

namespace YoutubeStreamBot
{
    public class LocalTest
    {
        public static void Test()
        {
            List<string> profiles = new List<string>(){
                @"C:\Users\ubt\AppData\Local\Google\Chrome\User Data\Profile 3",
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
