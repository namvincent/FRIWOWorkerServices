using Newtonsoft.Json;
using System.Collections.Generic;

namespace FRIWOServerApi.Data.TRACE
{
    public class JSONEvent
    {
        /// <summary>
        /// Each event must have a unique id which is used message tracking
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// A timestamp which tells when the event was created (performed) on the production line
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("dateTime")]
        public string DateTime { get; set; }

        /// <summary>
        /// A dictionary of identifiers for the product or component for which the event was sent
        /// </summary>
        [JsonProperty("fingerprint")]
        public Dictionary<string, string> Fingerprint { get; set; }
        /// <summary>
        /// Dictionary of properties which tells a state about the product or component.
        /// </summary>
        [JsonProperty("state")]
        public Dictionary<string, string> State { get; set; }
        /// <summary>
        /// Special flags to backend services
        /// </summary>
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        /// <summary>
        /// Structure in which assembly or disassembly of sub-components may be registered
        /// </summary>
    }
}
