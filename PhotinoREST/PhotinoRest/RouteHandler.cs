using System.Diagnostics;
using System.Net;
using System.Text;
using Photino.NET;

namespace PhotinoRest;

public class RouteHandler {

    public PhotinoWindow BoundWindow { get; private set; }
    private readonly List<Router> _routers = [];
    public IEnumerable<Router> Routers => _routers;

    public RouteHandler(PhotinoWindow boundWindow) {
        BoundWindow = boundWindow;
        BoundWindow.RegisterWebMessageReceivedHandler(WebMessageHandler);
    }

    internal void RegisterRouter(Router router) {
        router.SetupRoutes();
        _routers.Add(router);
    }

    private void WebMessageHandler(object? sender, string message) {
        Debug.WriteLine($"Message Received: {message}");
        if(sender == null) {
            Debug.WriteLine("Window is null :(");
            return;
        }
        var window = (PhotinoWindow)sender;

        var req = Req.Parse(message, window);

        //if(req.Path == "@pr.generatebindings") {
        //    Debug.WriteLine("GENERATE BINDINGS!");
        //    var bindings = PR.GenerateBindings(BoundWindow, false);
        //    BoundWindow.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) => {
        //        contentType = "text/javascript";
        //        return new MemoryStream(Encoding.UTF8.GetBytes(bindings));
        //    });
        //    return;
        //}

        var res = new Res(window, req.Headers with { Status = HttpStatusCode.OK });

        var router = _routers.FirstOrDefault(r => r.ContainsRoute(req.Headers.Method + req.Path));
        if(router == null) {
            // Route doesn't exist, test if route exists with other method
            if(_routers.FirstOrDefault(r => r.MethodNotAllowed(req.Path)) != null) {
                res.Send(HttpStatusCode.MethodNotAllowed, "Not Allowed");
                return;
            }
            res.Send(HttpStatusCode.NotFound, "Not Found");
            return;
        }
        var route = router.GetRoute(req.Headers.Method + req.Path);
        route.Execute(router, req, res);
    }

}

public static class StaticRouteHandler {

    private static Dictionary<Guid, RouteHandler> _handlerBindings = [];

    public static RouteHandler? GetHandler(this PhotinoWindow window) {
        _handlerBindings.TryGetValue(window.Id, out var handler);
        return handler;
    }

    public static RouteHandler? GetRouteHandler(this PhotinoWindow window) => GetHandler(window);

    public static PhotinoWindow RegisterRouter<TImplementation>(this PhotinoWindow window) where TImplementation : Router, new() {
        Debug.WriteLine($"Adding Window: {window.Id}");
        if(_handlerBindings.ContainsKey(window.Id)) {
            _handlerBindings[window.Id].RegisterRouter(new TImplementation());
            return window;
        }
        var handler = new RouteHandler(window);
        _handlerBindings.Add(window.Id, handler);
        handler.RegisterRouter(new TImplementation());
        return window;
    }
}