namespace Provausio.Practices.EventSourcing.Deserialization
{
    public interface IEventDeserializationFactory<TIdType>
    {
        bool TryDeserialize(byte[] eventData, byte[] eventMetadata, out EventInfo<TIdType> e);
    }
}