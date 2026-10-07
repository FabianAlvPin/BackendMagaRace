using System;
using System.Collections.Generic;

namespace BackendMagaRace.Models
{
    public class QualifierEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
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

    // Clasificación solo por potencia (ver CarData.cs en Unity, debe coincidir 1:1):
    // Street <=180 HP, Sport 181-260, Touring 261-400, Super 401-600, Hyper 600+.
    // Rally/Classic quedan deliberadamente afuera por ahora: no son tramos de potencia,
    // son otro eje (tracción/terreno, antigüedad) que se agregará aparte si hace falta.
    public enum CarCategory
    {
        Any = 0,
        Street = 1,
        Sport = 2,
        Touring = 3,
        Super = 4,
        Hyper = 5
    }

    public enum TransmissionType
    {
        Automatic = 0,
        Manual = 1
    }
}