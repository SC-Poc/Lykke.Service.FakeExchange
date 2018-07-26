namespace Lykke.Service.FakeExchange.Settings
{
    public sealed class RabbitMqSettings
    {
        public PublishSettings OrderBooks { get; set; }
        public PublishSettings TickPrices { get; set; }
    }
}
