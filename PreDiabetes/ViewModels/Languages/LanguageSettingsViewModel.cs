using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;

namespace PreDiabetes.ViewModels.Languages
{
    public partial class LanguageItem
    {
        public string Culture { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    public partial class LanguageSettingsViewModel : ObservableObject
    {
        const string PrefKey = "AppLanguage";

        public ObservableCollection<LanguageItem> Languages { get; } = new();

        [ObservableProperty]
        LanguageItem? selectedLanguage;

        bool _isInitializing;

        public LanguageSettingsViewModel()
        {
            _isInitializing = true;

            Languages.Add(new LanguageItem { Culture = "pt-PT", DisplayName = "Português (PT)" });
            Languages.Add(new LanguageItem { Culture = "en-US", DisplayName = "English (EN)" });

            var saved = Preferences.Get(PrefKey, "pt-PT");
            SelectedLanguage = Languages.FirstOrDefault(l => saved.StartsWith(l.Culture.Split('-')[0], StringComparison.OrdinalIgnoreCase))
                               ?? Languages.First();

            _isInitializing = false;
        }

        partial void OnSelectedLanguageChanged(LanguageItem? value)
        {
            if (value == null) return;

            // Do not auto-apply during construction
            if (_isInitializing) return;

            // If value matches saved preference, do nothing
            var saved = Preferences.Get(PrefKey, "pt-PT");
            if (saved.StartsWith(value.Culture.Split('-')[0], StringComparison.OrdinalIgnoreCase))
                return;

            // Apply when user actually changes selection
            _ = ApplyLanguageAsync(value.Culture);
        }

        async Task ApplyLanguageAsync(string cultureName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cultureName)) return;

                Preferences.Set(PrefKey, cultureName);

                var ci = new CultureInfo(cultureName);
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    try
                    {
                        Application.Current.MainPage = new AppShell();
                    }
                    catch
                    {
                        // TODO : log?
                    }
                });
            }
            catch
            {
                // TODO: swallow - no UI here; callers may show feedback if needed
            }
        }

        [RelayCommand]
        Task Apply()
        {
            if (SelectedLanguage != null)
                return ApplyLanguageAsync(SelectedLanguage.Culture);
            return Task.CompletedTask;
        }
    }
}
