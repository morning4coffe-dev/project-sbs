using Microsoft.UI.Xaml.Media.Imaging;

namespace Recurrents.Models;

public class User(string name, string email, BitmapImage? photo)
{
    public string Name { get; } = name;

    public string Email { get; } = email;

    public BitmapImage? Photo { get; set; } = photo;
}
