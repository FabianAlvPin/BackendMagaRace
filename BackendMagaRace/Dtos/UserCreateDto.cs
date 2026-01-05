namespace BackendMagaRace.Dtos
{
    public class UserCreateDto
    {
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = ""; // opcional si quieres hash
    }
}
