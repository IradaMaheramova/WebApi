namespace CityInfo.API.Sevices
{
    public interface IMailService
    {
        void Send(string subject, string message);
    }
}