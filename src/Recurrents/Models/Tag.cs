using System.Drawing;

namespace Recurrents.Models;

public class Tag(
    int id,
    string name,
    Color? color)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public Color? Color { get; set; } = color;
}
