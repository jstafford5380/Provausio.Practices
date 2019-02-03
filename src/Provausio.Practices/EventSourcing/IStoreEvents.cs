using System;
using System.Threading.Tasks;

namespace Provausio.Practices.EventSourcing
{
    public interface IStoreEvents<TIdType> : IDisposable
    {
        Task AppendEventsAsync(string streamName, params EventInfo<TIdType>[] infos);

        Task<EventStreamResult<TIdType>> GetEvents(string streamname, int startingPosition);

        Task<EventInfo<TIdType>> GetSnapshot(string streamName);

        Task Connect();

        void Disconnect();
    }
}