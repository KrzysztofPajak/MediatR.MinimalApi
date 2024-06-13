namespace WebApplication.Models;

public record User(string FirstName, string LastName, string Email)
{
    public Guid Id { get; set; } = new Guid();
}
