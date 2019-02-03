using System.Text;
using Newtonsoft.Json;

namespace Provausio.Practices.EventSourcing.Deserialization
{
    public class ByJsonTypeStrategy<TIdType> : EventDeserializationStrategy<TIdType>
    {
        protected override EventInfo<TIdType> Deserialize(byte[] eventData, byte[] eventMetadata)
        {
            return (EventInfo<TIdType>)JsonConvert.DeserializeObject(
                Encoding.UTF8.GetString(eventData),
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}
