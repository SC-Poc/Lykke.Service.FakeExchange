using Lykke.HttpClientGenerator;

namespace Lykke.Service.FakeExchange.Client
{
    public class FakeExchangeClient : IFakeExchangeClient
    {
        //public IControllerApi Controller { get; }
        
        public FakeExchangeClient(IHttpClientGenerator httpClientGenerator)
        {
            //Controller = httpClientGenerator.Generate<IControllerApi>();
        }
        
    }
}
