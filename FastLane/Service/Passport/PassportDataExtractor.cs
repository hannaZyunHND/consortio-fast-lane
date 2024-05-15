using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FastLane.Service.Passport
{
    public class PassportDataExtractor : IPassportDataExtractor
    {
        private readonly HttpClient _client;

        public PassportDataExtractor(HttpClient client)
        {
            _client = client;
        }

        public async Task ExtractPassportDataAsync(byte[] passportImage)
        {
            try
            {
                string apiUrl = "https://way2go.hnedu.com/process_image_passport";

                var formData = new MultipartFormDataContent();
                formData.Add(new ByteArrayContent(passportImage), "image", "passport_image.jpg");

                var response = await _client.PostAsync(apiUrl, formData);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);


                    if (responseData.success == true)
                    {
                        string[] base64Strings = responseData.converted_files_base64.ToObject<string[]>();

                        for (int i = 0; i < base64Strings.Length; i++)
                        {
                            byte[] bytes = Convert.FromBase64String(base64Strings[i]);
                            string base64String = Convert.ToBase64String(bytes);
                            Console.WriteLine($"Base64 data: {base64String}");
                        }

                    }
                    else
                    {
                        Console.WriteLine("Lỗi khi chuyển đổi: " + responseData.error);
                    }
                }
                else
                {
                    Console.WriteLine($"Lỗi khi gửi yêu cầu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }

}
