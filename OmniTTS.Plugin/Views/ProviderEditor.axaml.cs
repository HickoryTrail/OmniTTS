using Avalonia.Controls;
using OmniTTS.Plugin.ViewModels;
using OmniTTS.Shared;

namespace OmniTTS.Plugin;

public partial class ProviderEditor : UserControl
{
    public ProviderEditor(Provider provider)
    {
        InitializeComponent();
        var viewModel = DataContext as ProviderEditorViewModel
            ?? throw new InvalidOperationException("Unable to create the provider editor view model.");
        viewModel.Provider = provider;
        viewModel.Load();
    }
}
