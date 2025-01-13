namespace Recurrents.Models;

public class FilterCategory(
    string text, 
    string? glyph, 
    //Func<ItemViewModel, bool>? selector = null, 
    CategoryVisibility visibility = CategoryVisibility.Both)
{
    public string Text { get; } = text;
    public string? Glyph { get; } = glyph;
    //public Func<ItemViewModel, bool>? Selector { get; } = selector;
    public CategoryVisibility Visibility { get; } = visibility;
}
