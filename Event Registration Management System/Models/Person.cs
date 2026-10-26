namespace EventRegistrationManagementSystem.Models
{
    public interface IDisplayable
    {
        string GetDetails();
    }

    public abstract class Person : IDisplayable
    {
        public string FullName { get; set; } = string.Empty;

        public abstract string GetDetails();
    }
}
