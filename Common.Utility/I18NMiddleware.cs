using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;

namespace Common.Utility
{
    public static class I18NMiddleware
    {
        private static ILogger logger = new LoggerFactory().CreateLogger(MethodBase.GetCurrentMethod()!.Name);

        public static IApplicationBuilder UseI18NMiddleware(this IApplicationBuilder app)
        {
            return app.Use(async (ctx, next) =>
            {
                var finalLanguage = "en-US";
                try
                {
                    var sr = ctx.Request.Headers["Accept-Language"];

                    var firstlanguage = sr.ToString().Split(",").FirstOrDefault();

                    if (string.Equals(firstlanguage, "ja", StringComparison.OrdinalIgnoreCase) || string.Equals(firstlanguage, "ja-jp", StringComparison.OrdinalIgnoreCase))
                    {
                        finalLanguage = "ja-JP";
                    }
                    else if (string.Equals(firstlanguage, "de", StringComparison.OrdinalIgnoreCase) || firstlanguage!.StartsWith("de-", StringComparison.OrdinalIgnoreCase))
                    {
                        finalLanguage = "de-DE";
                    }
                    else if (string.Equals(firstlanguage, "en-gb", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(firstlanguage, "en-au", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(firstlanguage, "en-ca", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(firstlanguage, "en-in", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(firstlanguage, "en-nz", StringComparison.OrdinalIgnoreCase)
                          || string.Equals(firstlanguage, "en-za", StringComparison.OrdinalIgnoreCase)
                          || firstlanguage.StartsWith("en-gb-", StringComparison.OrdinalIgnoreCase))
                    {
                        finalLanguage = "en-GB";
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message, e);
                }

                CultureInfo culture = new CultureInfo(finalLanguage);
                try
                {
                    if (ctx.Request.Cookies.TryGetValue(CookieRequestCultureProvider.DefaultCookieName, out var value))
                    {
                        string result = CookieRequestCultureProvider.ParseCookieValue(value)?.Cultures?.FirstOrDefault().ToString()!;
                        if (!string.Equals(result, finalLanguage, StringComparison.OrdinalIgnoreCase))
                        {
                            ctx.Response.Cookies.Append(
                                CookieRequestCultureProvider.DefaultCookieName,
                                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None }
                            );
                        }

                    }
                    else
                    {
                        ctx.Response.Cookies.Append(
                                        CookieRequestCultureProvider.DefaultCookieName,
                                        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                        new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.None }
                                    );
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message, e);
                }
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                await next();
            });
        }
    }
}