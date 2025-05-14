using System.Text;
using System.Text.Json;
using Lab8.Entities;
using Microsoft.Extensions.Configuration;

namespace Lab8
{
    public class ShopApi
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ShopApi()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile(@"C:\Code\Volgatech\Quality\Lab8\JsonFiles\apiEndpoints.json", optional: false)
                .Build();

            _httpClient = new()
            {
                BaseAddress = new Uri(_config["ApiBaseUrl"])
            };
        }


        public async Task<ProductDto> GetProductById( int id )
        {
            List<ProductDto> prodcuts = await GetProductsAsync();

            return prodcuts?.Where(p => p.id == id.ToString()).FirstOrDefault();
        }

        public async Task<ResponseContent> AddProductAsync( ProductDto product )
        {
            StringContent stringContent = SerializeObject(product);

            var response = await _httpClient.PostAsync(_config["AddProductEndpoint"], stringContent);

            string respContent = await response.Content.ReadAsStringAsync();

            return DeserializeResponse(respContent);
        }

        public async Task<ResponseContent> EditProductAsync(ProductDto product)
        {
            StringContent stringContent = SerializeObject(product);

            var response = await _httpClient.PostAsync(_config["EditProductEndpoint"], stringContent);

            string respContent = await response.Content.ReadAsStringAsync();

            return DeserializeResponse(respContent);
        }

        public async Task<ResponseContent> DeleteProductByIdAsync( int id )
        {
            var response = await _httpClient.GetAsync(_config["RemoveProductEndpoint"] + id);

            string respContent = await response.Content.ReadAsStringAsync();

            return DeserializeResponse(respContent);
        }

        private async Task<List<ProductDto>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync(_config["GetProductsEndpoint"]);

            List<ProductDto> products = await GetJsonFromResponse<List<ProductDto>>(response);

            return products;
        }

        private static async Task<T> GetJsonFromResponse<T>(HttpResponseMessage response) where T : class
        {
            string respContent = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(respContent))
            {
                throw new Exception("Ответ от сервера пуст");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(respContent);
            }
            catch (JsonException ex)
            {
                throw new Exception("Не удалось десериализовать JSON", ex);
            }
        }

        private static StringContent SerializeObject( object obj )
        {
            var json = JsonSerializer.Serialize(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return content;
        }

        private static ResponseContent DeserializeResponse(string json)
        {
            try
            {
                var result = JsonSerializer.Deserialize<ResponseContent>(json);
                return result;
            }
            catch (JsonException)
            {
                return new ResponseContent { 
                    id = null,
                    status = ShopApiStatus.BadRequest,
                    error = json
                };
            }
        }
    }
}
