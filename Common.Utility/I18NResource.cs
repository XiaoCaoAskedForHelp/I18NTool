using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Common.Utility
{
    public static class I18NResource
    {
        private static readonly Dictionary<string, IStringLocalizer> _resources = new Dictionary<string, IStringLocalizer>();
        private static readonly Dictionary<string, string> _resourceCache = new Dictionary<string, string>();
        private static readonly ResourceNamesCache _resourceNamesCache = new ResourceNamesCache();

        public static void InitResource(Type type, params string[] baseNameList)
        {
            var assembly = type.Assembly;
            foreach (var baseName in baseNameList)
            {
                _resources[baseName] = new ResourceManagerStringLocalizer(new ResourceManager(baseName, assembly), assembly, baseName, _resourceNamesCache,new LoggerFactory().CreateLogger(type.FullName!));
            }
        }

        public static LocalizedString GetString(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            foreach (var item in _resources)
            {
                var result = item.Value[key];
                if (!string.IsNullOrEmpty(result) && result != key)
                    return result;
            }
            return new LocalizedString(key, key);
        }

        public static LocalizedString GetString(string key, CultureInfo cultureInfo)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var beforeCulture = System.Threading.Thread.CurrentThread.CurrentUICulture;
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
                foreach (var item in _resources)
                {
                    var result = item.Value[key];
                    if (!string.IsNullOrEmpty(result) && result != key)
                        return result;
                }
                return new LocalizedString(key, key);
            }
            catch (Exception e)
            {
                throw new ArgumentNullException(nameof(key));
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = beforeCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = beforeCulture;
            }
        }

        public static LocalizedString GetString(string key, params object[] args)
        {
            return new LocalizedString(key, string.Format(GetString(key), args));
        }

        private const string TestI18N = "testi18n";
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static string GetJsonResource(HttpContext context, string baseName)
        {
            var testi18n = context.Request.Headers["testi18n"];
            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            string key = $"{baseName}_{currentCulture}";

            if (string.Equals(testi18n.ToString(), TestI18N, StringComparison.OrdinalIgnoreCase))
            {
                var jObject = new JObject();
                foreach (var resource in _resources)
                {
                    if (!string.IsNullOrEmpty(baseName) && !string.Equals(resource.Key, baseName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    foreach (var item in resource.Value.GetAllStrings())
                    {
                        jObject.Add(item.Name, $"*{item.Value}");
                    }
                }
                return jObject.ToString();
            }
            else
            {
                if (!_resourceCache.TryGetValue(key, out var result))
                {
                    var jObject = new JObject();
                    foreach (var resource in _resources)
                    {
                        if (!string.IsNullOrEmpty(baseName) && !string.Equals(resource.Key, baseName, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        foreach (var item in resource.Value.GetAllStrings())
                        {
                            jObject.Add(item.Name, item.Value);
                        }
                    }
                    _resourceCache[key] = result = jObject.ToString();
                }
                return result;

            }
        }
    }
}
