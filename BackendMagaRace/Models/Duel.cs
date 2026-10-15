using System;
using System.Collections.Generic;
using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Models
{
    public class Duel
    {
        public Guid Id { get; set; }

        // Quien inició el desafío (no necesariamente quien reporta el resultado:
        // si se desconecta antes de terminar, Photon promueve a otro Master Client).
        public Guid CreatorUserId { get; set; }

        public DuelStatus Status { get; set; } = DuelStatus.Locked;

        // Monto fijo (una de las 3 apuestas: baja/media/alta) que pagó cada participante.
        public decimal WagerUsdt { get; set; }

        // Nombre de la sala de Photon usada para la carrera, para trazabilidad.
        public string RoomName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }

        public List<DuelParticipant> Participants { get; set; } = new();
    }
}
