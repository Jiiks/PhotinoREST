using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Photino.NET;

namespace PhotinoRest;

public record class PrHeaders(
    [property: JsonPropertyName("status")]
    HttpStatusCode Status, 
    [property: JsonPropertyName("time")]
    string Time, 
    [property: JsonPropertyName("version")]
    string Version, 
    [property: JsonPropertyName("method")]
    Method Method
);

public struct Req {

    [JsonPropertyName("headers")]
    public PrHeaders Headers { get; set; }
    [JsonPropertyName("path")]
    public string Path { get; set; }
    [JsonPropertyName("body")]
    public JsonObject Body { get; set; }
    public PhotinoWindow Host;
    public readonly PhotinoWindow Window => Host;

    public static Req Parse(string json, PhotinoWindow window) {
        var r = JsonSerializer.Deserialize<Req>(json);
        r.Host = window;
        return r;
    }

}