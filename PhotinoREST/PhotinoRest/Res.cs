using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Photino.NET;

namespace PhotinoRest;

public struct Res(PhotinoWindow host, PrHeaders headers) {
    private PrHeaders _headers = headers;
    public Res SetStatus(HttpStatusCode statusCode) {
        _headers = _headers with { Status = statusCode };
        return this;
    }

    [JsonPropertyName("headers")]
    public PrHeaders Headers { get => _headers; set => _headers = value;}
    [JsonPropertyName("body")]
    public Object? Body { get; set; }

    private readonly PhotinoWindow _host = host;

    public string Serialize() => JsonSerializer.Serialize(this);
    public void Send(object message) {
        Body = message;
        _host.SendWebMessage(Serialize());
    }

    public void Send(HttpStatusCode statusCode, object message) {
        _headers = _headers with { Status = statusCode };
        Send(message);
    }
}
