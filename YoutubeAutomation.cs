using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V106.Emulation;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Remote;

namespace YoutubeStreamBot
{
    public class YoutubeAutomation
    {
        private static Dictionary<string, int> _stats = new Dictionary<string, int>(){
            {"views", 0},
            {"waste", 0},
            {"starting", 0},
            {"navigating", 0},
            {"searching", 0}
        };


        private string Id;
        private ProxyPlugin _proxyPlugin;
        private bool useRemoteDriver;
        private IWebDriver _driver;
        private string _linkToStream;
        private GridInfo _gridInfo;
        private int ScrollCount = 5;
        private int _index;

        private static void Increase(string key)
        {
            lock (_stats)
            {
                _stats[key]++;
            }
        }

        private static void Decrease(string key)
        {
            lock (_stats)
            {
                _stats[key]--;
            }
            ShowStats();
        }

        private static void ShowStats()
        {
            Console.Clear();
            Console.WriteLine($"Views: {_stats["views"]}");
            Console.WriteLine($"Waste: {_stats["waste"]}");
            Console.WriteLine($"Starting: {_stats["starting"]}");
            Console.WriteLine($"Navigating: {_stats["navigating"]}");
            Console.WriteLine($"Searching: {_stats["searching"]}");
        }

        private readonly string? _chromeProfile;

        private readonly string? _proxyPluginPath;
        public YoutubeAutomation(ProxyPlugin proxy, GridInfo? grid, string? searchText, string? linkToStream, bool useRemote, int index, string? chromeProfile = null)
        {
            _proxyPlugin = proxy;
            SearchText = searchText;
            Id = Guid.NewGuid().ToString();
            useRemoteDriver = useRemote;
            _linkToStream = linkToStream;
            _chromeProfile = chromeProfile;
            _gridInfo = grid;
            _index = index;
            _proxyPluginPath = proxy.GetZippedPugin(index, true);
            // _proxyPluginPath = "/Users/ubt/Documents/Projects/YoutubeStreamBot/proxy.zip";
        }

        public string SearchText { get; set; }

        public void Run()
        {
            if (useRemoteDriver)
            {
                Increase("starting");

                if (_gridInfo.Browser == "chrome")
                {
                    var d = new ChromeOptions();
                    d.AddExtension(_proxyPluginPath);
                    d.PlatformName = _gridInfo.Platform;
                    //headless
                    _driver = new RemoteWebDriver(new Uri(_gridInfo.Url), d);
                }
                else if (_gridInfo.Browser == "edge")
                {
                    var d = new EdgeOptions();
                    d.AddExtension(_proxyPluginPath);
                    d.PlatformName = _gridInfo.Platform;
                    _driver = new RemoteWebDriver(new Uri(_gridInfo.Url), d);
                }
            }
            else
            {
                Increase("starting");
                // Console.WriteLine($"[{Id}] Starting...");
                ChromeOptions options = new ChromeOptions();
                //create driver service
                var driverService = ChromeDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                driverService.SuppressInitialDiagnosticInformation = true;
                if (_chromeProfile != null)
                {
                    options.AddArgument($"--user-data-dir={_chromeProfile}");
                }
                // options.AddExtension(_proxyPluginPath);
                _driver = new ChromeDriver(driverService, options);

                // Console.WriteLine($"[{Id}] Started");
            }


            if (_driver is RemoteWebDriver driver)
            {
                Id = driver.SessionId.ToString();
            }
            //216.158.201.0
            Decrease("starting");
            Increase("navigating");
            _driver.Navigate().GoToUrl("https://www.youtube.com/watch?v=YBJ0Zsg9Cn8");
            Thread.Sleep(10000);
            StartRandomCursor();
            StartRandomScroll();
            StartVideoPlayWatcher();
            return;
            // Console.WriteLine($"[{Id}] Navigating...");
            _driver.Navigate().GoToUrl("https://youtube.com");
            Thread.Sleep(20000);
            Decrease("navigating");
            Increase("searching");
            var search = _driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/div/ytd-masthead/div[3]/div[2]/ytd-searchbox/form/div[1]/div[1]/input"));
            search.SendKeys(SearchText);
            search.SendKeys(Keys.Enter);
            Thread.Sleep(20000);
            var videos = _driver.FindElements(By.XPath("//a[@id='video-title']"));
            //linkToStream to link id
            var linkId = _linkToStream.Split("/").Last();

            IWebElement? video = null;

            for (int i = 0; i <= ScrollCount; i++)
            {
                video = videos.FirstOrDefault(x => x.GetAttribute("href").Contains(linkId));

                if (video != null)
                {
                    break;
                }

                File.WriteAllText(Path.Combine("sessions", $"session_{Id}.txt"), Id);

                ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, 2000000)");
                Thread.Sleep(10000);
            }

            Decrease("searching");

            if (video != null)
            {
                Increase("views");
                // Console.WriteLine($"[{Id}] Clicking on {video.GetAttribute("href")}");
                video.Click();
            }
            else
            {
                Increase("waste");
                // Console.WriteLine($"[{Id}] Video not found");
            }

            StartRandomCursor();
            StartRandomScroll();
            StartVideoPlayWatcher();
        }

        public void Stop()
        {
            _driver?.Quit();
        }

        private void StartRandomCursor()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(new Random().Next(5000, 10000));
                    var x = new Random().Next(0, 1000);
                    var y = new Random().Next(0, 1000);
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"window.moveTo({x}, {y})");
                }
            });
        }

        private void StartRandomScroll()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (new Random().Next(0, 100) > 50)
                    {
                        ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, 2000000)");
                    }

                    //random 5-10 sec sleep
                    Thread.Sleep(new Random().Next(5000, 10000));

                    //scroll back to top
                    ((IJavaScriptExecutor)_driver).ExecuteScript("window.scrollTo(0, 0)");
                    Thread.Sleep(new Random().Next(5000, 50000));
                }
            });
        }

        private void StartVideoPlayWatcher()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    try
                    {
                        PlayVideo();
                    }
                    catch (Exception e)
                    {
                        // Console.WriteLine(e.Message);
                    }
                }
            });
        }

        private void PlayVideo()
        {
            var videosElement = _driver.FindElement(By.CssSelector("#movie_player > div.html5-video-container > video"));

            if (videosElement != null)
            {
                //check if video is playing
                if (videosElement.GetAttribute("paused") == "true")
                {
                    Console.WriteLine($"[{Id}] Video is paused");
                    //click play button
                    var playButton = _driver.FindElement(By.CssSelector("#movie_player > div.ytp-chrome-bottom > div.ytp-chrome-controls > div.ytp-left-controls > button"));
                    playButton.Click();
                    Console.WriteLine($"[{Id}] Clicked play button");
                }
                else
                {
                    // Console.WriteLine($"[{Id}] Video is playing");
                }
            }
            else
            {
                Console.WriteLine("Video element not found");
            }
        }
    }
}
