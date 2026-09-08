namespace BackendMagaRace.Services.Interfaces
{
    public record TransbankCommitResult(
        bool IsAuthorized,
        string Status,
        decimal? Amount,
        string? AuthorizationCode,
        string? PaymentTypeCode,
        int? ResponseCode);

    public interface ITransbankService
    {
        (string Token, string FormUrl) CreateTransaction(string buyOrder, string sessionId, decimal amount, string returnUrl);
        TransbankCommitResult CommitTransaction(string token);
    }
}
