using Newtonsoft.Json;

namespace ViewPointReader.Core.Models
{
    public class VprKeyPhraseContent
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
