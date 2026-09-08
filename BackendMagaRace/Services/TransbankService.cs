using BackendMagaRace.Options;
using BackendMagaRace.Services.Interfaces;
using Microsoft.Extensions.Options;
using Transbank.Webpay.WebpayPlus;

namespace BackendMagaRace.Services
{
    public class TransbankService : ITransbankService
    {
        private readonly Transaction _transaction;

        public TransbankService(IOptions<TransbankOptions> options)
        {
            var o = options.Value;
            _transaction = o.IsProduction
                ? Transaction.buildForProduction(o.CommerceCode, o.ApiKey)
                : Transaction.buildForIntegration(o.CommerceCode, o.ApiKey);
        }

        public (string Token, string FormUrl) CreateTransaction(string buyOrder, string sessionId, decimal amount, string returnUrl)
        {
            try
            {
                var response = _transaction.Create(buyOrder, sessionId, amount, returnUrl);
                return (response.Token, response.Url);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"No se pudo iniciar la transacción en Transbank: {ex.Message}");
            }
        }

        public TransbankCommitResult CommitTransaction(string token)
        {
            try
            {
                var response = _transaction.Commit(token);
                var isAuthorized = response.Status == "AUTHORIZED" && response.ResponseCode == 0;
                return new TransbankCommitResult(
                    isAuthorized,
                    response.Status,
                    response.Amount,
                    response.AuthorizationCode,
                    response.PaymentTypeCode,
                    response.ResponseCode);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"No se pudo confirmar la transacción en Transbank: {ex.Message}");
            }
        }
    }
}
