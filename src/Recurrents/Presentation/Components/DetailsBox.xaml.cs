namespace Recurrents.Presentation.Components;

public sealed partial class DetailsBox : UserControl
{
	public DetailsBox()
	{
		this.InitializeComponent();
	}

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register(
      nameof(HeaderText),
      typeof(string),
      typeof(DetailsBox),
      new PropertyMetadata("")
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      nameof(Text),
      typeof(string),
      typeof(DetailsBox),
      new PropertyMetadata(null)
    );

    public new UIElement Content
    {
        get => (UIElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
      nameof(Content),
      typeof(UIElement),
      typeof(DetailsBox),
      new PropertyMetadata(null)
    );
}
