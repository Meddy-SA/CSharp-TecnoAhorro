namespace TecnoCredito.Services.Interfaces;

public interface IRazorViewRenderer
{
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
}
