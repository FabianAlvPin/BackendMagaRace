using System;
using System.Collections.Generic;

namespace BackendMagaRace.Dtos.Duel
{
    public class CreateDuelDto
    {
        // Debe ser exactamente uno de los 3 valores configurados en DuelOptions
        // (Low/Mid/HighWagerUsdt) o 0 si el desafío es sin apuesta.
        public decimal WagerUsdt { get; set; }

        // Todos los que van a correr, incluyendo al que desafía. Entre 2 y
        // DuelOptions.MaxParticipants.
        public List<Guid> ParticipantUserIds { get; set; } = new();

        public string RoomName { get; set; } = string.Empty;
    }
}
