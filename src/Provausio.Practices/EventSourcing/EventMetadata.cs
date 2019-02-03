namespace Provausio.Practices.EventSourcing
{
    public class EventMetadata : JsonEventData
    {
        /// <summary>
        /// The full type name.
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// The assembly qualified type name.
        /// </summary>
        public string AssemblyQualifiedType { get; set; }

        /// <summary>
        /// Declared namespaces
        /// </summary>
        public string EventNamespaces { get; set; }
    }
}