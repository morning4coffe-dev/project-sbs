using Microsoft.UI.Xaml.Markup;
using Recurrents.Presentation.Helpers;

namespace Recurrents.Presentation.CustomMarkupExtensions;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
internal class Localize : MarkupExtension
{
    public string? Name { get; set; }

    protected override object ProvideValue() =>
        string.IsNullOrEmpty(Name)
        ? string.Empty
        : Name.Localize();
}
