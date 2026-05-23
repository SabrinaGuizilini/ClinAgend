using MudBlazor;

namespace ClinAgend.Web.Themes
{
    public static class CustomMudTheme
    {
        public static MudTheme DefaultTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#1C79BC",
                Secondary = "#2DD4BF",
                Tertiary = "#6EC1E4",
                Background = "#F5F7FA",
                AppbarBackground = "#FCFAFA",
                TextPrimary = "#244B6A",
                TextSecondary = "#244B6A",
                Success = "#2DD4BF",
                Warning = "#FFA000",
                Error = "#E53935",
                DrawerBackground = "#FCFAFA",
            },
            Typography = new Typography()
            {
                Default = new DefaultTypography()
                {
                    FontFamily = new[] { "Open Sans", "sans-serif" }
                },
                H1 = new H1Typography()
                {
                    FontFamily = new[] { "Montserrat", "sans-serif" },
                    FontWeight = "600",
                    FontSize = "2rem",
                    LineHeight = "1.3"
                },
                Button = new ButtonTypography()
                {
                    FontFamily = new[] { "Montserrat", "sans-serif" },
                    FontWeight = "500",
                    TextTransform = "none",
                    FontSize = "16px"
                }
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "8px"
            }
        };
    }
}
