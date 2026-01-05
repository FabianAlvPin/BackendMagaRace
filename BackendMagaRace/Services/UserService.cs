using BackendMagaRace.Data;
using BackendMagaRace.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BackendMagaRace.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }
        public class DomainException : Exception
        {
            public int StatusCode { get; }

            public DomainException(string message, int statusCode = 400)
                : base(message)
            {
                StatusCode = statusCode;
            }
        }


        // Crear usuario con wallet y password hash
        public async Task<User> CreateAsync(string email, string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
                throw new DomainException("EMAIL_EXISTS");

            if (await _context.Users.AnyAsync(u => u.Username == username))
                throw new DomainException("USERNAME_EXISTS");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = HashPassword(password),
                Wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }


        // Buscar usuario por Id
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Wallet)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Buscar usuario por username
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Wallet)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // Verificar password
        public bool VerifyPassword(string password, string storedHash)
        {
            // Simple SHA256 hash (temporal)
            using var sha256 = SHA256.Create();
            var hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return hash == storedHash;
        }

        // Generar hash de password
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
