using System;
using System.Collections.Generic;
using DAS.Infrastructure.Logging;

namespace Provausio.Practices.EventSourcing.Deserialization
{
    public class EventDeserializationFactory<TIdType> : IEventDeserializationFactory<TIdType>
    {
        private readonly List<IEventDeserializationStrategy<TIdType>> _deserializationStrategies = new List<IEventDeserializationStrategy<TIdType>>();

        private EventDeserializationFactory(
            ByAttributeStrategy<TIdType> byAttributeStrategy, 
            AttributeAsAssemblyQualifiedNameStrategy<TIdType> attributeAsAssemblyQualifiedNameStrategy)
        {
            // in order of likeliness
            _deserializationStrategies.Add(byAttributeStrategy);
            _deserializationStrategies.Add(new ByJsonTypeStrategy<TIdType>());
            _deserializationStrategies.Add(new ByFullTypeNameStrategy<TIdType>());
            _deserializationStrategies.Add(new ByAssemblyQualifiedTypeNameStrategy<TIdType>());
            _deserializationStrategies.Add(attributeAsAssemblyQualifiedNameStrategy);
        }

        public EventDeserializationFactory(string eventLibraryPath)
            : this(new ByAttributeStrategy<TIdType>(eventLibraryPath), 
                   new AttributeAsAssemblyQualifiedNameStrategy<TIdType>(eventLibraryPath)) { }

        public EventDeserializationFactory(params Type[] typeRefs)
            : this(new ByAttributeStrategy<TIdType>(typeRefs), 
                   new AttributeAsAssemblyQualifiedNameStrategy<TIdType>(typeRefs)) { }

        public bool TryDeserialize(byte[] eventData, byte[] eventMetaData, out EventInfo<TIdType> e)
        {
            e = null;
            foreach (var strategy in _deserializationStrategies)
            {
                try
                {
                    e = strategy.DeserializeEvent(eventData, eventMetaData);
                    if (e != null)
                    {
                        //Logger.Debug($"Deserialization succeeded using {strategy} ({e.GetType()})", this);
                        return true;
                    }
                }
                catch { }
            }

            Logger.Error("All deserialization strategies failed.", this);
            return false;
        }
    }
}