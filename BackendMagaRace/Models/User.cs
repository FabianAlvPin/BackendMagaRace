using System;

namespace BackendMagaRace.Models
{
    public class User
    {
        public Guid Id { get; set; }

        // Login
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // Datos públicos
        public string Username { get; set; } = null!;

        // Estado
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        public Wallet Wallet { get; set; } = null!;
    }
}
