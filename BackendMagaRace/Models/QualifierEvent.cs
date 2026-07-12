using System;
using System.Collections.Generic;

namespace BackendMagaRace.Models
{
    public class QualifierEvent
    {
        public Guid Id { get; set; }

        public Guid TrackId { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        // Valor que paga cada jugador para participar
        public decimal EntryCost { get; set; }

        // Premio garantizado por el organizador
        public decimal BasePrize { get; set; }

        // ==========================
        // REGLAS DEL EVENTO
        // ==========================

        // Sentido de la pista
        public TrackDirection Direction { get; set; }

        // Categoría permitida
        public CarCategory CarCategory { get; set; }

        // Tipo de transmisión
        public TransmissionType Transmission { get; set; }

        // Cantidad de vueltas
        public int Laps { get; set; }

        public bool IsClosed { get; set; }

        public Track Track { get; set; } = null!;

        public ICollection<QualifierSession> Sessions { get; set; }
            = new List<QualifierSession>();

        public ICollection<QualifierEntry> Entries { get; set; }
            = new List<QualifierEntry>();

        public ICollection<QualifierPrize> Prizes { get; set; }
            = new List<QualifierPrize>();
    }

    public enum TrackDirection
    {
        Normal = 0,
        Reverse = 1
    }

    public enum CarCategory
    {
        Any = 0,
        Street = 1,
        Sport = 2,
        Super = 3,
        Hyper = 4,
        Rally = 5,
        Classic = 6
    }

    public enum TransmissionType
    {
        Automatic = 0,
        Manual = 1
    }
}