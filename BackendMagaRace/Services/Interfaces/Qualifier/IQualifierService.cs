using BackendMagaRace.Dtos.OnlineRace;
using BackendMagaRace.Dtos.Qualifier;
using BackendMagaRace.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendMagaRace.Services.Interfaces
{
    public interface IQualifierService
    {

        // Eventos

        Task<object> GetActiveEvents();

        Task<object?> GetEvent(Guid eventId);


        Task<QualifierEvent> CreateEvent(
           CreateQualifierEventDto dto);


        Task CloseEvent(Guid eventId);

        Task<object?> GetPlayerPosition(
    Guid eventId,
    Guid userId);

        // Sesiones
        Task<QualifierSession?> GetActiveSession(
           Guid userId,
           Guid eventId);
        Task<object> StartSession(
            Guid userId,
            Guid qualifierEventId);



        Task<SubmitLapResponse> SubmitLap(
            Guid userId,
            Guid sessionId,
            int timeMs);



        Task<object> GetSession(
            Guid sessionId);



        // Ranking

        Task<object> GetRanking(
            Guid eventId);



        // Resultados

        Task<object> GetResults(
            Guid eventId);

        Task<QualifierSession> Join(
    Guid userId,
    Guid qualifierEventId);

    }
}