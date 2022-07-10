using HtmlAgilityPack;

namespace Application.Scraper
{
    public abstract class BaseScraper
    {
        private readonly HttpClient _httpClient;

        protected BaseScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<string> GetHtmlStringAsync(string url)
        {
            return await _httpClient.GetStringAsync(url);
        }

        protected async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            var html = await GetHtmlStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            return htmlDoc;
        }

        protected async Task<IEnumerable<HtmlDocument>> GetHtmlDocuments(IEnumerable<string> urls)
        {
            var htmlDocs = new List<HtmlDocument>();

            foreach (var url in urls)
            {
                var doc = await GetHtmlDocument(url);
                htmlDocs.Add(doc);
            }

            return htmlDocs;
        }
    }
}
