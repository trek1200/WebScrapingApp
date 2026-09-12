using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using WebScraipingApp.Models;

namespace WebScraipingApp.Services
{
    public class SourceAnalysisService : ISourceAnalysisService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<SourceAnalysisResult> AnalyzeAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("URLを入力してください。", nameof(url));
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("有効なURL(http/https)を入力してください。", nameof(url));
            }

            var html = await httpClient.GetStringAsync(uri);

            return AnalyzeHtml(url, html);
        }

        // Kept separate from HTTP so parsing can be regression-tested offline.
        internal static SourceAnalysisResult AnalyzeHtml(string url, string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            // Script/style contents are source code, not page text or HTML elements.
            foreach (var node in document.DocumentNode.Descendants()
                .Where(node => node.Name == "script" || node.Name == "style").ToList())
            {
                node.Remove();
            }

            var root = document.DocumentNode;
            var title = root.Descendants("title").FirstOrDefault();
            var description = root.Descendants("meta").FirstOrDefault(node =>
                string.Equals(node.GetAttributeValue("name", string.Empty),
                    "description", StringComparison.OrdinalIgnoreCase));
            var textRoot = root.Descendants("body").FirstOrDefault() ?? root;
            var text = string.Join(" ", textRoot.Descendants()
                .Where(node => node.NodeType == HtmlNodeType.Text &&
                    !node.Ancestors("head").Any())
                .Select(node => WebUtility.HtmlDecode(((HtmlTextNode)node).Text)));

            return new SourceAnalysisResult
            {
                Url = url,
                Title = title == null ? string.Empty : WebUtility.HtmlDecode(title.InnerText).Trim(),
                MetaDescription = description == null ? string.Empty :
                    WebUtility.HtmlDecode(description.GetAttributeValue("content", string.Empty)).Trim(),
                LinkCount = root.Descendants("a").Count(),
                ImageCount = root.Descendants("img").Count(),
                HeadingCount = root.Descendants().Count(node =>
                    node.NodeType == HtmlNodeType.Element &&
                    node.Name.Length == 2 && node.Name[0] == 'h' &&
                    node.Name[1] >= '1' && node.Name[1] <= '6'),
                WordCount = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries).Length,
                // Preserve the existing metric: length of the downloaded HTML source.
                CharacterCount = html.Length,
            };
        }
    }
}
