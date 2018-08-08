using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.FakeExchange.Client 
{
    /// <summary>
    /// FakeExchange client settings.
    /// </summary>
    public class FakeExchangeServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
