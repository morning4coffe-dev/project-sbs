namespace Recurrents.Models;

public class NavigationCategory(
    uint id,
    string text,
    string? glyph,
    Type page,
    CategoryVisibility visibility = CategoryVisibility.Both)
{
    public uint Id { get; } = id;
    public string Text { get; } = text;
    public string? Glyph { get; } = glyph;
    public Type Page { get; } = page;
    public CategoryVisibility Visibility { get; } = visibility;
}
