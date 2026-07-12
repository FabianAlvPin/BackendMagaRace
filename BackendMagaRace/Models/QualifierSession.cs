using BackendMagaRace.Models;

public class QualifierSession
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid QualifierEventId { get; set; }


    public DateTime CreatedAt { get; set; }


    public DateTime ActiveUntil { get; set; }


    public int? BestLapMs { get; set; }


    public User User { get; set; } = null!;


    public QualifierEvent Event { get; set; } = null!;
}