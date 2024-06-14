using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq;

namespace PhotinoRest;

public class Route(MethodInfo methodInfo) {

    private readonly MethodInfo _methodInfo = methodInfo;

    public MethodInfo GetMethodInfo => _methodInfo;

    public RouteAttribute RouteAttribute { get; private set; } = methodInfo.GetCustomAttribute<RouteAttribute>()!;

    public void Execute(Router router, Req req, Res res) {
        _methodInfo.Invoke(router, [req, res]);
    }

    public string GenerateBinding(Router router) {
        var paramList = (from param in _methodInfo.GetBaseDefinition().GetParameters()
                         select $"{param.ParameterType} {param.Name}").ToList();
        var basePath = router.BasePath;
        var fullPath = $"{basePath}{RouteAttribute.Path}";

        var declaringName = _methodInfo.DeclaringType!.Name;
        var declaringNameSpace = _methodInfo.DeclaringType!.Namespace;
        var returnType = _methodInfo.GetBaseDefinition().ReturnType;
        var name = _methodInfo.GetBaseDefinition().Name;
        return $$"""

            // {{declaringNameSpace}}.{{declaringName}} {{returnType}} {{name}}({{string.Join(", ", paramList)}})
            // Base Path: {{basePath}}, Full Path: {{fullPath}};
            // [Route(Path: {{RouteAttribute.Path}}, Aync: {{RouteAttribute.Async}}, Method: {{RouteAttribute.Method}})]
            static async {{name}}(params) {
                const res = await PhotinoRest.{{RouteAttribute.Method}}("{{fullPath}}", params);
                // Middleware goes here
                return res;
            }

        """;
    }

}
