# PhotinoREST

Super simple REST api with automatic binding generation for Photino.NET. https://www.tryphotino.io/

> Note: still in prototype phase.

Example:

Add the `photinorest.js` to your frontend.

Create new `PhotinoWindow` and register routers and bindings.

```cs
string windowTitle = "PhotinoREST Example";
var window = new PhotinoWindow()
    .SetTitle(windowTitle)
    .SetUseOsDefaultSize(false)
    .SetSize(new Size(1280, 720))
    .Center()
    .RegisterRouter<ExampleRouter>() // Register our router
    .RegisterRouter<ExampleRouterTwo>() // You can have more than one router
    .RegisterRestBindings("R") // auto register bindings to ui as R. Register all your routes before doing so.
    // Method name is used for binding.
    .Load("wwwroot/index.html");
window.WaitForClose(); // Starts the application event loop
```
Example router:

```cs
public class ExampleRouter : Router {

	public override string BasePath  =>  "/";
	
	[Route(Path = "test", Async = true, Method = Method.GET)]
	public async void MyRoute(Req req, Res  res) {
		await Task.Delay(500);
		res.Send("Example Router /test");
	}
}
```

Calling from client side:
```js
async function Test() {
	const res = await R.MyRoute(params);
}
```

```
MIT License

Copyright (c) 2024 Alexei Stukov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
