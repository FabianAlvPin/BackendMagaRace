public class CreateQualifierPrizeDto
{
    public Guid QualifierEventId { get; set; }

    public int FromPosition { get; set; }

    public int ToPosition { get; set; }

    public decimal PrizePercent { get; set; }
}