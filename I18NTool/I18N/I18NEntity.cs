using Common.Utility;
using Microsoft.Extensions.Localization;

namespace I18NTool.I18N
{
    public static class I18NEntity
    {
        private const string I18N_BASENAME = "I18NTool.I18n.Demo.Web";
        static I18NEntity()
        {
            I18NResource.InitResource(typeof(I18NEntity), I18N_BASENAME);
        }
        public static LocalizedString GetString(string key)
        {
            return I18NResource.GetString(key);
        }
        public static LocalizedString GetString(string key, params object[] args)
        {
            return I18NResource.GetString(key, args);
        }
        public static string GetJsonResource(HttpContext context)
        {
            return I18NResource.GetJsonResource(context, I18N_BASENAME);
        }
    }
}
