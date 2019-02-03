using System;
using Newtonsoft.Json;

namespace Provausio.Practices.DDD
{
    public abstract class EntityState
    {
        /// <summary>
        /// The ID of the aggregate from which this state was generated.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Returns JSON representation of the aggregate state.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}