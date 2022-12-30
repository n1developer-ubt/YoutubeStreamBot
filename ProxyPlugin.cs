using System;
using System.IO.Compression;
using RestSharp.Authenticators.OAuth;

namespace YoutubeStreamBot
{
    public class ProxyPlugin
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public ProxyPlugin(string host, int port, string username, string password)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }

        private const string men = @"{
    ""version"": ""1.0.0"",
    ""manifest_version"": 2,
    ""name"": ""Chrome Proxy"",
    ""permissions"": [
        ""proxy"",
        ""tabs"",
        ""unlimitedStorage"",
        ""storage"",
        ""<all_urls>"",
        ""webRequest"",
        ""webRequestBlocking""
    ],
    ""background"": {
        ""scripts"": [""background.js""]
    },
    ""minimum_chrome_version"":""22.0.0""
}
";

        private const string background = @"var config = {
        mode: ""fixed_servers"",
        rules: {
        singleProxy: {
            scheme: ""http"",
            host: ""{{host}}"",
            port: parseInt({{port}})
        },
        bypassList: [""localhost""]
        }
    };

chrome.proxy.settings.set({value: config, scope: ""regular""}, function() {});

function callbackFn(details) {
    return {
        authCredentials: {
            username: ""{{username}}"",
            password: ""{{password}}""
        }
    };
}

chrome.webRequest.onAuthRequired.addListener(
            callbackFn,
            {urls: [""<all_urls>""]},
            ['blocking']
);";

        public string GetZippedPugin(int id, bool increasePort = false)
        {
            var p = increasePort ? Port + id : Port;
            Console.WriteLine("Id: " + id + " Port: " + p);
            File.WriteAllText(Path.Join("proxyplugin", "manifest.json"), men);
            var tbackground = background.Replace("{{host}}", Host);
            tbackground = tbackground.Replace("{{port}}", p + "");
            tbackground = tbackground.Replace("{{username}}", Username);
            tbackground = tbackground.Replace("{{password}}", Password);
            File.WriteAllText(Path.Join("proxyplugin", "background.js"), tbackground);

            var outPath = Path.Join("plugins", (increasePort ? Port + id : Port) + "_proxy.zip");

            //create if directory not existing plugins
            if (!Directory.Exists("plugins"))
            {
                Directory.CreateDirectory("plugins");
            }

            if (File.Exists(outPath))
                File.Delete(outPath);

            ZipFile.CreateFromDirectory(Path.Join("proxyplugin"), outPath);

            return outPath;
        }

    }
}
