using PreDiabetes.Pages.Languages;

namespace PreDiabetes
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("PreDiabetesPage", typeof(PreDiabetesPage));
            Routing.RegisterRoute("LanguageSettingsPage", typeof(LanguageSettingsPage));

        }

    }
}
