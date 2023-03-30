using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Common.Utility
{
    public static class ServiceCollectionExtensions
    {
        //public static IApplicationBuilder UseI18NEndpoint(this IApplicationBuilder app, string uri, Func<HttpContext, string> getResource)
        //{
        //    app.UseEndpoints(endpoint =>
        //    {
        //        endpoint.MapGet(uri, async (requestContext) =>
        //        {
        //            requestContext.Response.ContentType = "application/javascript";
        //            await requestContext.Response.WriteAsync(getResource(requestContext));
        //            await requestContext.Response.Body.FlushAsync();
        //        });
        //    });
        //    return app;
        //}
    }
}
