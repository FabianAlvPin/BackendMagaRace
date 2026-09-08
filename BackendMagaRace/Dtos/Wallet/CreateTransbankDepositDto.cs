using BackendMagaRace.Models.Enums;

namespace BackendMagaRace.Dtos.Wallet
{
    public class CreateTransbankDepositDto
    {
        public decimal AmountClp { get; set; }
        public TransbankPaymentMethod Method { get; set; } = TransbankPaymentMethod.Credit;
    }
}
