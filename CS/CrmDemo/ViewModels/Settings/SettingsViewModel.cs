using Microsoft.EntityFrameworkCore;

using DevExpress.Maui.Core;
using CrmDemo.DataLayer;
using CrmDemo.DataModel.Models;
using CrmDemo.ViewModels.Common;

namespace CrmDemo.ViewModels.Settings;

public class SettingsViewModel : BindableBase {
    private readonly UserSessionService sessionService;

    private Employee currentUser;
    public Employee CurrentUser {
        get => currentUser;
        set => SetValue(ref currentUser, value);
    }

    private string language;
    public string Language {
        get => language;
        set => SetValue(ref language, value);
    }

    private string previewColorName;
    public string PreviewColorName {
        get => previewColorName;
        set => SetValue(ref previewColorName, value);
    }

#if ANDROID
    private int selectedColorIndex = 1;
#else
    private int selectedColorIndex;
#endif
    public int SelectedColorIndex {
        get => selectedColorIndex;
        set => SetValue(ref selectedColorIndex, value);
    }

    public List<ColorModel> Items { get; set; }
    public List<string> Languages { get; set; }

    public SettingsViewModel(UserSessionService sessionService) {
        this.sessionService = sessionService;
        Languages = new List<string>() {
            "English", "Español", "Français", "Deutsch", "Русский",
        };
        Language = Languages.First();
        Items = new List<ColorModel>() {
#if ANDROID
            new ColorModel(Colors.Black, "System Color", true),
#endif
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.Purple), ThemeSeedColor.Purple.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.Violet), ThemeSeedColor.Violet.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.Red), ThemeSeedColor.Red.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.Brown), ThemeSeedColor.Brown.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.TealGreen), ThemeSeedColor.TealGreen.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.Green), ThemeSeedColor.Green.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.DarkGreen), ThemeSeedColor.DarkGreen.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.DarkCyan), ThemeSeedColor.DarkCyan.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.DeepSeaBlue), ThemeSeedColor.DeepSeaBlue.ToString()),
            new ColorModel(ThemeManager.GetSeedColor(ThemeSeedColor.Blue), ThemeSeedColor.Blue.ToString()),
        };
    }

    public void ChangeColor(ColorModel colorModel) {
        if (colorModel == null)
            return;

        PreviewColorName = colorModel.DisplayName;
        if (colorModel.IsSystemColor) {
            ThemeManager.UseAndroidSystemColor = true;
            return;
        }

        ThemeManager.UseAndroidSystemColor = false;
        ThemeManager.Theme = new Theme(colorModel.Color);
    }
    public Task LoadDataAsync() {
        return Task.Run(() => LoadData());
    }
    public void Refresh() {
        var oldValue = CurrentUser;
        CurrentUser = null;
        CurrentUser = oldValue;
    }

    private void LoadData() {
        using (CrmContext crmContext = new CrmContext()) {
            CurrentUser = crmContext.Employees
                .Where(e => e.Id == sessionService.CurrentUserId)
                .Include(c => c.Avatar).ThenInclude(a => a.FullImage)
                .Include(c => c.Avatar).ThenInclude(a => a.ThumbnailImage)
                .First();
        }
    }
}

public class ColorModel {
    public Color Color { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public bool IsSystemColor { get; set; }

    public ColorModel(Color color, string displayName, bool isSystemColor = false) {
        Color = color;
        DisplayName = displayName;
        IsSystemColor = isSystemColor;
        Name = isSystemColor ? "System" : string.Empty;
    }
}