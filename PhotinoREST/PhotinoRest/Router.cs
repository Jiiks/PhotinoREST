using System.Reflection;

namespace PhotinoRest;

public abstract class Router {
    //Base path for routing.
    public abstract string BasePath { get; }

    //All routes defined in the router
    public Dictionary<string, Route> Routes { get; } = new();
    private List<string> _rawRoutes = new();

    internal void SetupRoutes() {
        var routes = GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(RouteAttribute), false).Length > 0);
        foreach (var route in routes) {
            var routeAttribute = route.GetCustomAttribute<RouteAttribute>();
            Routes.Add(routeAttribute?.Method + BasePath + routeAttribute?.Path, new Route(route));
            _rawRoutes.Add(BasePath + routeAttribute?.Path);
        }
    }

    //Does a route exist in the router
    public bool ContainsRoute(string path) => Routes.ContainsKey(path);
    public bool MethodNotAllowed(string path) => _rawRoutes.Contains(path);
    public Route GetRoute(string path) => Routes[path];

    public IEnumerable<string> GenerateBindings() => 
        Routes.Values.Select(route => route.GenerateBinding(this));

}