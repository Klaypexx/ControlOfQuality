using Lab8.Entities;
using Microsoft.Extensions.Configuration;

namespace Lab8
{
    public class ApiProductTests
    {
        private static readonly IConfiguration _validProductTests;
        private static readonly IConfiguration _invalidProductTests;
        private static readonly IConfiguration _aliasProductTests;
        private readonly ShopApi _shopApi = new();

        static ApiProductTests()
        {
            _validProductTests = new ConfigurationBuilder()
                .AddJsonFile(@"C:\Code\Volgatech\Quality\Lab8\JsonFiles\validProductTests.json", optional: false)
                .Build();

            _invalidProductTests = new ConfigurationBuilder()
                .AddJsonFile(@"C:\Code\Volgatech\Quality\Lab8\JsonFiles\invalidProductTests.json", optional: false)
                .Build();

            _aliasProductTests = new ConfigurationBuilder()
                .AddJsonFile(@"C:\Code\Volgatech\Quality\Lab8\JsonFiles\aliasProductTests.json", optional: false)
                .Build();
        }

        public static IEnumerable<object[]> ValidProductTestCases()
        {
            IConfigurationSection section = _validProductTests.GetSection("valid");
            foreach (var child in section.GetChildren())
            {
                yield return new object[] { child.Key, child.Get<ProductDto>() };
            }
        }

        public static IEnumerable<object[]> InvalidProductTestCases()
        {
            IConfigurationSection section = _invalidProductTests.GetSection("invalid");
            foreach (var child in section.GetChildren())
            {
                yield return new object[] { child.Key, child.Get<ProductDto>() };
            }
        }

        [Theory]
        [MemberData(nameof(ValidProductTestCases))]
        public async Task AddProductAsync_SendValidCases_ReturnsSuccessAndId(string caseName, ProductDto expectedProduct)
        {
            ResponseContent response = new();
            try
            {
                response = await _shopApi.AddProductAsync(expectedProduct);
                Assert.True(response.status == ShopApiStatus.Success,
                    $"Кейс '{caseName}': Запрос выполнен с ошибкой: {response.error} ");

                if (response.status == ShopApiStatus.Success)
                {
                    ProductDto actualProduct = await _shopApi.GetProductById(response.id.Value);
                    Assert.True(actualProduct != null, $"Кейс '{caseName}': Объект по id пустой \n\n Ожидаемый объект {expectedProduct}");
                    CompareProducts(expectedProduct, actualProduct);
                }
            }
            finally
            {
                await CleanupProduct(response.id);
            }
        }

        [Theory]
        [MemberData(nameof(InvalidProductTestCases))]
        public async Task AddProductAsync_SendInvalidCases_ReturnsBadRequest(string caseName, ProductDto expectedProduct)
        {
            ResponseContent response = new();
            try
            {
                response = await _shopApi.AddProductAsync(expectedProduct);
                Assert.True(response.status == ShopApiStatus.BadRequest,
                    $"Кейс '{caseName}': Запрос выполнен успешно (ожидался ответ с ошибкой)");

                if ( response.status == ShopApiStatus.Success )
                {
                    ProductDto actualProduct = await _shopApi.GetProductById(response.id.Value);
                    Assert.True(actualProduct == null, $"Кейс '{caseName}': Объект пришел не пустым");
                }
            }
            finally
            {
                await CleanupProduct(response.id);
            }
        }

        [Fact]
        public async Task AddProductAsync_SendTwoValidProductsWithTheSameTitle_ReturnsAliasWithPrefix()
        {
            ResponseContent response = new();
            List<int> productsId = [];
            try
            {
                IConfigurationSection validProductSection = _aliasProductTests.GetSection("validProduct");
                ProductDto expectedValidProduct = validProductSection.Get<ProductDto>();

                IConfigurationSection correctAliasSection = _aliasProductTests.GetSection("correctAlias");
                string correctAlias = correctAliasSection.Get<string>();

                response = await _shopApi.AddProductAsync(validProductSection.Get<ProductDto>());
                Assert.True(response.status == ShopApiStatus.Success, $"Запрос выполнен с ошибкой: {response.error}");
                productsId.Add(response.id.Value);

                response = await _shopApi.AddProductAsync(validProductSection.Get<ProductDto>());
                Assert.True(response.status == ShopApiStatus.Success, $"Запрос выполнен с ошибкой: {response.error}");
                productsId.Add(response.id.Value);

                ProductDto actualProduct = await _shopApi.GetProductById(response.id.Value);
                Assert.True(actualProduct != null, $"Объект по id пришел пустым после создания");

                Assert.True(actualProduct.alias == correctAlias, "Пришел некорректный alias");
            }
            finally
            {
                if (productsId.Count > 0 )
                {
                    foreach ( var id in productsId )
                    {

                        await CleanupProduct(id);
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidProductTestCases))]
        public async Task EditProductAsync_EditWithValidValues_ReturnsSuccessAfterEdit(string caseName, ProductDto newProduct)
        {
            ResponseContent response = new();
            try
            {
                IConfigurationSection validProductSection = _validProductTests.GetSection("validProduct");
                ProductDto validProduct = validProductSection.Get<ProductDto>();

                // Создаем объект с валидными данными
                response = await _shopApi.AddProductAsync(validProduct);
                Assert.True(response.status == ShopApiStatus.Success,
                    $"Кейс '{caseName}': Запрос выполнен с ошибкой: {response.error}");

                // Получаем созданный объект по id
                ProductDto actualProduct = await _shopApi.GetProductById(response.id.Value);
                Assert.True(actualProduct != null, $"Кейс '{caseName}': Объект по id пустой \n\n Ожидаемый объект {validProduct}");

                // Подставляем полученный id новому объекту для редактирования, пробуем послать запрос
                newProduct.id = response.id.Value.ToString();
                ResponseContent responseAfterEdit = await _shopApi.EditProductAsync(newProduct);
                Assert.True(response.status == ShopApiStatus.Success,
                   $"Кейс '{caseName}': Запрос выполнен с ошибкой: {response.error}");

                // Пробуем получить продукт после редактирования
                actualProduct = await _shopApi.GetProductById(response.id.Value);
                Assert.True(actualProduct != null, $"Кейс '{caseName}': Объект по id пришел пустым после редактирования \n\n Ожидаемый объект {newProduct}");

                CompareProducts(newProduct, actualProduct);
            }
            finally
            {
                await CleanupProduct(response.id);
            }
        }

        [Theory]
        [MemberData(nameof(InvalidProductTestCases))]
        public async Task EditProductAsync_EditWithInvalidValues_ReturnsBadRequest(string caseName, ProductDto newProduct)
        {
            ResponseContent response = new();
            try
            {
                IConfigurationSection validProductSection = _validProductTests.GetSection("validProduct");
                ProductDto validProduct = validProductSection.Get<ProductDto>();

                // Создаем объект с валидными данными
                response = await _shopApi.AddProductAsync(validProduct);
                Assert.True(response.status == ShopApiStatus.Success,
                    $"Кейс '{caseName}': Запрос выполнен с ошибкой: {response.error}");

                // Получаем созданный объект по id
                ProductDto actualProduct = await _shopApi.GetProductById(response.id.Value);
                Assert.True(actualProduct != null, $"Кейс '{caseName}': Объект по id пустой \n\n Ожидаемый объект {validProduct}");

                // Подставляем полученный id новому объекту для редактирования, пробуем послать запрос
                newProduct.id = response.id.Value.ToString();
                ResponseContent responseAfterEdit = await _shopApi.EditProductAsync(newProduct);
                Assert.True(responseAfterEdit.status == ShopApiStatus.BadRequest,
                   $"Кейс '{caseName}': Запрос выполнен успешно (ожидался ответ с ошибкой)");

                // Пробуем получить продукт после редактирования
                ProductDto productAfterEdit = await _shopApi.GetProductById(response.id.Value);
                Assert.True
                    (productAfterEdit != null, $"Кейс '{caseName}': Объект не должен быть удален после невалидного редактирования" +
                    $" \n\n Ожидаемый объект {actualProduct}");

                CompareProducts(actualProduct, productAfterEdit);
            }
            finally
            {
                await CleanupProduct(response.id);
            }
        }

        [Theory]
        [MemberData(nameof(ValidProductTestCases))]
        public async Task DeleteProductByIdAsync_DeleteProduct_ReturnsSuccessAfterDelete(string caseName, ProductDto validProduct)
        {
            ResponseContent response = new();
            try
            {
                // Создаем объект с валидными данными
                response = await _shopApi.AddProductAsync(validProduct);
                Assert.True(response.status == ShopApiStatus.Success,
                    $"Кейс '{caseName}': Запрос выполнен с ошибкой: {response.error}");

                // Пробуем удалить
                ResponseContent responseAfterDelete = await _shopApi.DeleteProductByIdAsync(response.id.Value);
                Assert.True(response.status == ShopApiStatus.Success,
                   $"Кейс '{caseName}': Запрос выполнен с ошибкой: {response.error}");

                // Пробуем получить продукт после редактирования
                ProductDto actualProduct = await _shopApi.GetProductById(response.id.Value);
                Assert.True(actualProduct == null, $"Кейс '{caseName}': Объект по id не был удален");
            }
            finally
            {
                await CleanupProduct(response.id);
            }
        }

        [Theory]
        [MemberData(-1)]
        [MemberData(9999)]
        public async Task DeleteProductByIdAsync_DeleteByInvalidId_ReturnsBadRequestStatus(int invalidProviderId)
        {
            var response = await _shopApi.DeleteProductByIdAsync(invalidProviderId);
            Assert.Equal(ShopApiStatus.BadRequest, response.status);
        }

        private async Task CleanupProduct( int? id )
        {
            if (id != null)
            {
                await _shopApi.DeleteProductByIdAsync(id.Value);
            }
        }

        private void CompareProducts(ProductDto expected, ProductDto actual)
        {
            Assert.NotNull(actual);

            Assert.True(expected.category_id == actual.category_id, "поля category_id не совпадают");
            Assert.True(expected.title == actual.title, "поля title не совпадают");
            Assert.True(expected.content == actual.content, "поля content не совпадают");
            Assert.True(double.Parse(expected.price) == double.Parse(actual.price), "поля price не совпадают");
            Assert.True(double.Parse(expected.old_price) == double.Parse(actual.old_price), "поля old_price не совпадают");
            Assert.True(expected.status == actual.status, "поля status не совпадают");
            Assert.True(expected.keywords == actual.keywords, "поля keywords не совпадают");
            Assert.True(expected.description == actual.description, "поля description не совпадают");
            Assert.True(expected.hit == actual.hit, "поля hit не совпадают");
        }
    }
}