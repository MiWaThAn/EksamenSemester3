using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Services.Theme
{
    public class ThemeService
    {
        public string BackgroundColor
        {
            get => Preferences.Default.Get("BgColor", "#5b6b4e");
            private set => Preferences.Default.Set("BgColor", value);
        }

        public event Action? OnThemeChanged;

        public void UpdateBackgroundColor(string newColor)
        {
            BackgroundColor = newColor;
            OnThemeChanged?.Invoke();
        }
    }
}
