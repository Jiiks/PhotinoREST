namespace PhotinoRest;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class RouteAttribute : Attribute {
    public string Path { get; set; } = "/";
    public bool Async { get; set; } = true;
    public Method Method { get; set;} = Method.GET;
 }

public enum Method {
    GET,
    POST,
    PUT,
    DELETE
}