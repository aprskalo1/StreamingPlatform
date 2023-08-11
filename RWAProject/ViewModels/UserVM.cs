namespace RWAProject.ViewModels
{
    public class UserVM
    {
        public string Username { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public int CountryOfResidenceId { get; set; }
    }
}
