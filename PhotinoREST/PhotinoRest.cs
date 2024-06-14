using Photino.NET;
using System.Diagnostics;
using System.Text;

namespace PhotinoRest;

public static class PR {

    public static string GenerateBindings(this PhotinoWindow window, bool module = true, string className = "PR") {
        return GenerateBindings(window.GetHandler()?.Routers, module, className);
    }

    public static string GenerateBindings(IEnumerable<Router>? routers, bool module = true, string className = "PR") {
        if(routers == null) return $$"""
                                    export default class {
                                        static __NAME__ = "{{className}}";
                                    }
                                    (() => {
                                        console.error('Null Routers!');
                                    })();
                                    """;

        var bindings = from router in routers
               from binding in router.GenerateBindings()
               select binding;
        
        if(!module) {
            return $$"""
            class {{className}} {
                {{string.Join("", bindings)}}
            }
            
            {{className}} = {{className}};
            """;
        }

        return $$"""
        export default class {
            static __NAME__ = "{{className}}";
            {{string.Join("", bindings)}}
        }
        """;
    }

    public static PhotinoWindow RegisterRestBindings(this PhotinoWindow window, string prefix = "R") {
        window.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) => {
            var bindings = GenerateBindings(window, false, prefix);
            Debug.WriteLine(bindings);
            contentType = "text/javascript";
            return new MemoryStream(Encoding.UTF8.GetBytes(bindings));
        });
        return window;
    }
}
