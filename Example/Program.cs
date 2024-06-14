using Photino.NET;
using System.Drawing;
using PhotinoRest;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Net;
using Microsoft.Win32;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HelloPhotinoApp;
class Program {

    private readonly PhotinoWindow _mainWindow;

    [STAThread]
    static void Main(string[] args) {
        _ = new Program();
    }

    static bool GetDefaultApp(string extension, out string app)  {
        // Find the default app in the registry
        var key = Registry.ClassesRoot.OpenSubKey(extension);
        if(key == null) {
            app = "No default application set";
            return false;
        }
        var value = key.GetValue("");
        if(value == null) {
            app = "No default application set";
            return false;
        }
        var appKey = Registry.ClassesRoot.OpenSubKey(value.ToString() + "\\shell\\open\\command");
        if(appKey == null) {
            app = "No default application set";
            return false;
        }
        var appValue = appKey.GetValue("");
        if(appValue == null) {
            app = "No default application set";
            return false;
        }
        app = appValue.ToString();
        return true;
    }

    public Program() {
        string windowTitle = "PhotinoREST Example";
        _mainWindow = new PhotinoWindow()
            .SetTitle(windowTitle)
            .SetUseOsDefaultSize(false)
            .SetSize(new Size(1280, 720))
            .Center()
            .RegisterRouter<ExampleRouter>() // Register our router
            .RegisterRouter<ExampleRouterTwo>() // You can have more than one router
            .RegisterRestBindings("R") // auto register bindings to ui as R. Register all your routes before doing so.
            // Method name is used for binding.
            .Load("wwwroot/index.html");

        _mainWindow.WaitForClose(); // Starts the application event loop
    }

    public class ExampleRouter : Router {
        public override string BasePath => "/";

        [Route(Path = "test", Async = true, Method = Method.GET)]
        public async void Test(Req req, Res res) {
            await Task.Delay(500);
            res.Send("Example Router /test");
        }

        [Route(Path = "br")]
        public async void BadRequest(Req req, Res res) {
            await Task.Delay(100);
            res.Send(HttpStatusCode.BadRequest, "Bad Request!");
        }

        [Route(Path = "ofd")]
        public async void GetFolder(Req req, Res res) {
            
            await Task.Run(() => {
                var sof = req.Host.ShowOpenFolder();
            if(sof.Length > 0) {
                var folderName = sof[0];
                var dirs = Directory.GetDirectories(folderName);
                var files = Directory.GetFiles(folderName);
                var data = new{
                    root = folderName,
                    directories = dirs,
                    files
                };
                res.Send(data);
            }
            });
            Debug.WriteLine("OpenFolder finished");
            
        }

        [Route(Path = "movieinfo")]
        public async void GetMovieInfo(Req req, Res res) {
            await Task.Run(async() => {
                var sof = req.Host.ShowOpenFile();
                if(sof.Length > 0) {
                    var fi = new FileInfo(sof[0]);
                    var dummy = true;
                    // Using dummy fileinfo for example
                    var name = "";
                    var image = "";
                    var overview = "";
                    Dictionary<string, string> about = new();
                    Dictionary<string, string> links = new();
                    if(dummy) {
                        name = "The Lord of the Rings: The Fellowship of the Ring";
                        image = "https://i.imgur.com/c5v04RU.jpeg";
                        overview = "An ancient Ring thought lost for centuries has been found, and through a strange twist of fate has been given to a small Hobbit named Frodo. When Gandalf discovers the Ring is in fact the One Ring of the Dark Lord Sauron, Frodo must make an epic quest to the Cracks of Doom in order to destroy it. However, he does not go alone. He is joined by Gandalf, Legolas the elf, Gimli the Dwarf, Aragorn, Boromir, and his three Hobbit friends Merry, Pippin, and Samwise. Through mountains, snow, darkness, forests, rivers and plains, facing evil and danger at every corner the Fellowship of the Ring must go. Their quest to destroy the One Ring is the only hope for the end of the Dark Lords reign.";
                        about = new Dictionary<string, string> {
                            ["rating"] = "8.9/10",
                            ["director"] = "Peter Jackson",
                            ["writers"] = "J.R.R. Tolkien, Fran Walsh, Philippa Boyens",
                            ["stars"] = "Elijah Wood, Ian McKellen, Orlando Bloom",
                            ["genres"] = "Action, Adventure, Drama"
                        };
                        links = new Dictionary<string, string> {
                            ["imdb"] = "https://www.imdb.com/title/tt0120737/",
                            ["moviedb"] = "https://www.themoviedb.org/movie/120-the-lord-of-the-rings-the-fellowship-of-the-ring"
                        };
                    } else {
                        var url = "";
                        var body = "";

                        using var client = new HttpClient();
                        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(url, requestContent);
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var job = JsonNode.Parse(responseBody);
                        var foo = job["results"][0]["hits"][0];
                        name = foo["name"].ToString();
                        image = foo["image"].ToString();
                        overview = foo["overview"].ToString();
                    }

                    var data = new {
                        filepath = fi.FullName,
                        name,
                        image,
                        overview,
                        about = JsonSerializer.Serialize(about),
                        links = JsonSerializer.Serialize(links)
                    };

                    res.Send(data);
                }
            });
        }

        [Route(Path = "playmovie", Method = Method.POST)]
        public async void PlayMovie(Req req, Res res) {
            var foo = req.Body["path"].ToString();
            if(GetDefaultApp(".mkv", out var app)) {
                Process.Start(app.Replace("%1", foo));
            }
            res.Send("OK");
        }

    }

    public class ExampleRouterTwo : Router {
        public override string BasePath => "/two/";
        [Route(Path = "test", Async = true, Method = Method.GET)]
        public async void TestTwo(Req req, Res res) {
            await Task.Delay(100);
            res.Send("Example Router 2 /two/test");
        }

    }

}
