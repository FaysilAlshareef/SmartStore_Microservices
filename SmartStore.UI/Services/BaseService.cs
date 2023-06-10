using Newtonsoft.Json;
using SmartStore.UI.Dtos;
using SmartStore.UI.Models;
using SmartStore.UI.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace SmartStore.UI.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClient;

        public ResponseDto ResponseModel { get; set; }
        public BaseService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
            ResponseModel = new ResponseDto();
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            try
            {
                // Create HttpClient
                var client = _httpClient.CreateClient("SmartStore");
                // Create HttpRequestMessage
                var message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.ApiUrl);
                client.DefaultRequestHeaders.Clear();

                // check apiRequest.date 
                if (apiRequest.Data != null)
                {
                    //Serialize To string
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data),
                        Encoding.UTF8, "application/json");
                }
                if (!string.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Bearer" ,apiRequest.AccessToken
                        );

                }
                // Set HttpMethod Type
                switch (apiRequest.ApiType)
                {
                    case SD.ApiType.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case SD.ApiType.POST:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.PUT:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.DELETE:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                // Send Http Request
                var response = await client.SendAsync(message);

                // Read message Content as string
                var apiContent = await response.Content.ReadAsStringAsync();

                // Convert From string to Json
                var apiResponseDto = JsonConvert.DeserializeObject<T>(apiContent);

                //Return 
                return apiResponseDto;
            }
            catch (Exception e)
            {
                var dto = new ResponseDto
                {
                    IsSuccess = false,
                    DisplayMessage = "Error occurs",
                    ErrorMessages = new() { Convert.ToString(e.Message) }
                };
                //
                var result = JsonConvert.SerializeObject(dto);
                //
                var apiResponseDto = JsonConvert.DeserializeObject<T>(result);
                return apiResponseDto;
            }
        }
        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }
    }
}
