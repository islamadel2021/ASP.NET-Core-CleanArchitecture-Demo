using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;

namespace CRUDTest
{
    public class PersonsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        #region Index

        //[Fact]
        public async Task Index_ReturnView()
        {
            //Act
            HttpResponseMessage response = await _client.GetAsync("/persons/index");

            //Assert
            response.Should().BeSuccessful();
            string responseBody = await response.Content.ReadAsStringAsync();
            HtmlDocument html = new();
            html.LoadHtml(responseBody);
            var document = html.DocumentNode;
            document.QuerySelector("table").Should().NotBeNull();
        }

        #endregion
    }
}
