using Avalonia.Controls;
using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;
using ClassIsland.Shared;
using FluentAvalonia.UI.Controls;
using OmniTTS.Plugin.Services;
using OmniTTS.Plugin.ViewModels;
using OmniTTS.Shared;

namespace OmniTTS.Plugin;

[SettingsPageInfo("plugins.OmniTTS", "OmniTTS 设置", "\uED53", "\uED52", SettingsPageCategory.External)]
public partial class OmniTTSettingsPage : SettingsPageBase
{
    private readonly SettingsService _settingsService;

    public OmniTTSettingsPage()
    {
        InitializeComponent();
        _settingsService = IAppHost.GetService<SettingsService>()
            ?? throw new InvalidOperationException("SettingsService is unavailable.");
        SelectDefaultProvider();
    }

    private void SelectDefaultProvider()
    {
        foreach (var item in DefaultProviderComboBox.Items.OfType<ComboBoxItem>())
        {
            if (string.Equals(item.Tag?.ToString(), _settingsService.Setting.DefaultProvider.ToString(), StringComparison.Ordinal))
            {
                DefaultProviderComboBox.SelectedItem = item;
                return;
            }
        }
    }

    private void DefaultProviderComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DefaultProviderComboBox.SelectedItem is ComboBoxItem { Tag: { } tag } &&
            Enum.TryParse<Provider>(tag.ToString(), out var provider))
        {
            _settingsService.Setting.DefaultProvider = provider;
        }
    }

    private async void Provider_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not FASettingsExpanderItem { Tag: { } tag } ||
            !Enum.TryParse<Provider>(tag.ToString(), out var provider) ||
            provider == Provider.None)
        {
            return;
        }

        var editor = new ProviderEditor(provider);
        var dialog = new FATaskDialog
        {
            Header = provider.ToString(),
            SubHeader = $"编辑 {provider} 提供方设置",
            FooterVisibility = FATaskDialogFooterVisibility.Never,
            Content = editor,
            XamlRoot = TopLevel.GetTopLevel(this)
                ?? throw new InvalidOperationException("Unable to resolve the settings window."),
            Buttons =
            {
                FATaskDialogButton.OKButton,
                FATaskDialogButton.CancelButton
            }
        };

        if (object.Equals(await dialog.ShowAsync(true), FATaskDialogStandardResult.OK))
        {
            ((ProviderEditorViewModel)editor.DataContext!).Save();
        }
    }
}
