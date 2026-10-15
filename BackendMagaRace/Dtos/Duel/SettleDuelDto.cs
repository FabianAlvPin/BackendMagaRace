using System;
using System.Collections.Generic;

namespace BackendMagaRace.Dtos.Duel
{
    public class DuelResultDto
    {
        public Guid UserId { get; set; }

        // Null si no llegó a terminar (ej. desconectado); en ese caso Disconnected debe ser true.
        public int? FinalPosition { get; set; }

        public bool Disconnected { get; set; }
    }

    public class SettleDuelDto
    {
        public List<DuelResultDto> Results { get; set; } = new();
    }
}
