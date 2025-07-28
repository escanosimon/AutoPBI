using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AutoPBI.Controls;

public class ToggleButtonGroup : TemplatedControl
{
    public static readonly StyledProperty<int> Amount1Property = AvaloniaProperty.Register<ToggleButtonGroup, int>(
        nameof(Amount1));

    public int Amount1
    {
        get => GetValue(Amount1Property);
        set => SetValue(Amount1Property, value);
    }

    public static readonly StyledProperty<int> Amount2Property = AvaloniaProperty.Register<ToggleButtonGroup, int>(
        nameof(Amount2));

    public int Amount2
    {
        get => GetValue(Amount2Property);
        set => SetValue(Amount2Property, value);
    }
    
    public static readonly StyledProperty<bool> BooleanProperty = AvaloniaProperty.Register<ToggleButtonGroup, bool>(
        nameof(Boolean));

    public bool Boolean
    {
        get => GetValue(BooleanProperty);
        set => SetValue(BooleanProperty, value);
    }

    public static readonly StyledProperty<bool> NotBooleanProperty = AvaloniaProperty.Register<ToggleButtonGroup, bool>(
        nameof(NotBoolean));

    public bool NotBoolean
    {
        get => GetValue(NotBooleanProperty);
        set => SetValue(NotBooleanProperty, value);
    }

    public static readonly StyledProperty<ICommand> Command1Property = AvaloniaProperty.Register<ToggleButtonGroup, ICommand>(
        nameof(Command1));

    public ICommand Command1
    {
        get => GetValue(Command1Property);
        set => SetValue(Command1Property, value);
    }

    public static readonly StyledProperty<ICommand> Command2Property = AvaloniaProperty.Register<ToggleButtonGroup, ICommand>(
        nameof(Command2));

    public ICommand Command2
    {
        get => GetValue(Command2Property);
        set => SetValue(Command2Property, value);
    }

    public static readonly StyledProperty<string> Text1Property = AvaloniaProperty.Register<ToggleButtonGroup, string>(
        nameof(Text1));

    public string Text1
    {
        get => GetValue(Text1Property);
        set => SetValue(Text1Property, value);
    }

    public static readonly StyledProperty<string> Text2Property = AvaloniaProperty.Register<ToggleButtonGroup, string>(
        nameof(Text2));

    public string Text2
    {
        get => GetValue(Text2Property);
        set => SetValue(Text2Property, value);
    }
}