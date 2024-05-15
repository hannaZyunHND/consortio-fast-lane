namespace FastLane.Service.Email
{
    public interface IEmailService
    {
        Task SendPendingEmail(Dtos.Email.Email request);
        Task SendConfirmEmail(Dtos.Email.Email request);
        Task SendCompleteEmail(Dtos.Email.Email request);
        Task SendCancelEmail(Dtos.Email.Email request);
    }
}
