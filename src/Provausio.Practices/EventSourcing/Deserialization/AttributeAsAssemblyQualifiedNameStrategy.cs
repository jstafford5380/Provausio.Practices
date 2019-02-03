using System;
using DAS.Infrastructure.Ext;

namespace Provausio.Practices.EventSourcing.Deserialization
{
    /// <summary>
    /// Will use attribute to match on AssemblyQualifiedType found in the event metadata
    /// </summary>
    /// <seealso cref="ByAttributeStrategy{T}" />
    public class AttributeAsAssemblyQualifiedNameStrategy<TIdType> : ByAttributeStrategy<TIdType>
    {
        public AttributeAsAssemblyQualifiedNameStrategy(params Type[] typeRefs)
            : base(typeRefs) { }

        public AttributeAsAssemblyQualifiedNameStrategy(string path)
            : base(path) { }

        protected override EventInfo<TIdType> Deserialize(byte[] eventData, byte[] eventMetadata)
        {
            var metaData = eventMetadata.DeserializeJson<EventMetadata>();
            var eventNamespace = new EventNamespace(metaData.AssemblyQualifiedType);

            // use simplified assembly name to ignore revision/build
            var simpleNamespace = eventNamespace.GetSimpleNamespace();
            var dtoType = FindType(simpleNamespace);
            var deserialized = eventData.DeserializeJson<EventInfo<TIdType>>(dtoType);
            return deserialized;
        }
    }
}