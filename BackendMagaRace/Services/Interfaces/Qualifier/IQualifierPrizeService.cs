using BackendMagaRace.Models;
using System;
using System.Threading.Tasks;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IQualifierPrizeService
    {

        // Calcula el pozo disponible
        Task<decimal> CalculatePrizePool(
            Guid qualifierEventId);



        // Calcula premios según ranking
        Task CalculatePrizes(
            Guid qualifierEventId);



        // Entrega premios a wallets
        Task DistributePrizes(
            Guid qualifierEventId);



        // Consulta premios finales
        Task<object> GetPrizeResults(
            Guid qualifierEventId);
        Task<QualifierPrize> Create(CreateQualifierPrizeDto dto);
     

  
    }
}