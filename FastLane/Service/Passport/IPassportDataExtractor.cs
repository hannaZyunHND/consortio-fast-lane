using Newtonsoft.Json.Linq;

namespace FastLane.Service.Passport
{
    public interface IPassportDataExtractor
    {
        Task ExtractPassportDataAsync(byte[] passportImage);
    }

}
