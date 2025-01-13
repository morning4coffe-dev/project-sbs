namespace Recurrents.Presentation.Helpers;

internal static class LocalizationExtensions
{
    private static readonly IStringLocalizer s_localizer = App.Services?.GetRequiredService<IStringLocalizer>() 
        ?? throw new InvalidOperationException("Could not get Localizer from App.Services");

    public static string Localize(this string resourceKey) =>
        s_localizer.GetString(resourceKey);
}
