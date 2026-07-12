using System;
using System.Threading.Tasks;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IQualifierEntryService
    {

        Task<object> BuyEntry(
            Guid userId,
            Guid qualifierEventId);



        Task<object?> GetEntry(
            Guid userId,
            Guid qualifierEventId);


        Task<bool> HasValidEntry(
            Guid userId,
            Guid qualifierEventId);

    }
}