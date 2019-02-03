namespace Provausio.Practices.EventSourcing.Deserialization
{
    public interface IEventDeserializationStrategy<TIdType>
    {
        EventInfo<TIdType> DeserializeEvent(byte[] eventData, byte[] eventMetaData);
    }
}