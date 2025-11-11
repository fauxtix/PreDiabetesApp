using System.Globalization;
using System.Reflection;
using System.Resources;

namespace PreDiabetes.Resources.Languages
{
    [ContentProperty(nameof(Key))]
    public class LocalizeExtension : IMarkupExtension
    {
        public string Key { get; set; } = string.Empty;

        private static ResourceManager? s_resourceManager;

        private static ResourceManager? GetResourceManager()
        {
            if (s_resourceManager != null)
                return s_resourceManager;

            try
            {
                s_resourceManager = new ResourceManager("PreDiabetes.Resources.Languages.AppResources", typeof(LocalizeExtension).GetTypeInfo().Assembly);
            }
            catch
            {
                s_resourceManager = null;
            }

            return s_resourceManager;
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key))
                return string.Empty;

            try
            {
                var rm = GetResourceManager();
                if (rm is null)
                    return $"[{Key}]";

                var culture = CultureInfo.CurrentUICulture ?? CultureInfo.InvariantCulture;
                string? value;
                try
                {
                    value = rm.GetString(Key, culture);
                }
                catch (MissingManifestResourceException)
                {
                    value = null;
                }
                catch
                {
                    value = null;
                }

                return value ?? $"[{Key}]";
            }
            catch
            {
                return $"[{Key}]";
            }
        }
    }
}