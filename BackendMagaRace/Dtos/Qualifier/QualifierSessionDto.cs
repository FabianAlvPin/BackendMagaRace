using System;

namespace BackendMagaRace.Dtos.Qualifier
{
    public class QualifierSessionDto
    {
        public Guid Id { get; set; }
        public DateTime ActiveUntil { get; set; }

        // 0 = todavía no registra tiempo en este evento. No se usa int? acá a propósito:
        // Unity's JsonUtility no deserializa bien tipos Nullable, así que se evita por completo
        // mandar "null" por este campo.
        public int BestLapMs { get; set; }
    }
}