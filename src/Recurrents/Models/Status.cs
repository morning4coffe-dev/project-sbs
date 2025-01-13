namespace Recurrents.Models;

public enum State 
{
    Active,
    Archived
}

public record Status(State State, DateTime Date)
{
    public State State { get; set; } = State;
    public DateTime Date { get; set; } = Date;
}
