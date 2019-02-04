namespace Lykke.Service.FakeExchange.Domain
{
    public class WalletBalance
    {
        public string Asset { set; get; }
        public decimal Balance { set; get; }
        public decimal Reserved { set; get; }
    }
}
