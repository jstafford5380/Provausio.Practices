using System;
using System.Text;
using Newtonsoft.Json;

namespace Provausio.Practices.EventSourcing.Deserialization
{
    public class ByAssemblyQualifiedTypeNameStrategy<TIdType> : EventDeserializationStrategy<TIdType>
    {
        protected override EventInfo<TIdType> Deserialize(byte[] eventData, byte[] eventMetadata)
        {
            var metaData = JsonConvert.DeserializeObject<EventMetadata>(Encoding.UTF8.GetString(eventMetadata));
            if (string.IsNullOrEmpty(metaData?.AssemblyQualifiedType))
                throw new InvalidOperationException("Invalid or missing event metadata.");

            var type = Type.GetType(metaData.AssemblyQualifiedType);
            return (EventInfo<TIdType>)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventData), type);
        }
    }

    public class ByFullTypeNameStrategy<TIdType> : EventDeserializationStrategy<TIdType>
    {
        protected override EventInfo<TIdType> Deserialize(byte[] eventData, byte[] eventMetadata)
        {
            var metaData = JsonConvert.DeserializeObject<EventMetadata>(Encoding.UTF8.GetString(eventMetadata));
            if (string.IsNullOrEmpty(metaData?.AssemblyQualifiedType))
                throw new InvalidOperationException("Invalid or missing event metadata.");

            var type = Type.GetType(metaData.FullTypeName);
            return (EventInfo<TIdType>)JsonConvert.DeserializeObject(Encoding.UTF8.GetString(eventData), type);
        }
    }
}