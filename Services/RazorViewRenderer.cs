using System.Reflection;
using RazorLight;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class RazorViewRenderer : IRazorViewRenderer
{
    private readonly RazorLightEngine _engine;

    public RazorViewRenderer()
    {
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
    {
        try
        {
            string viewPath = Path.Combine("Views", viewName + ".cshtml");
            string result = await _engine.CompileRenderAsync(viewPath, model);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error rendering view {viewName}: {ex.Message}");
        }
    }
}
