using System;

namespace BackendMagaRace.Dtos
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public decimal Balance { get; set; }
    }
}
