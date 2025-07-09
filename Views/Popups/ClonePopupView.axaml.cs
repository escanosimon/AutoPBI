using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoPBI.Views.Popups;

public partial class ClonePopupView : UserControl
{
    public ClonePopupView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}