using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebScraipingApp.Models;

namespace WebScraipingApp.Services
{
    public class SourceAnalysisService : ISourceAnalysisService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static readonly Regex TitleRegex = new Regex(
            "<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex MetaDescriptionRegex = new Regex(
            "<meta[^>]+name=[\"']description[\"'][^>]+content=[\"'](.*?)[\"']",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex LinkRegex = new Regex("<a[\\s>]", RegexOptions.IgnoreCase);
        private static readonly Regex ImageRegex = new Regex("<img[\\s>]", RegexOptions.IgnoreCase);
        private static readonly Regex HeadingRegex = new Regex("<h[1-6][\\s>]", RegexOptions.IgnoreCase);
        private static readonly Regex TagRegex = new Regex("<[^>]*>", RegexOptions.Singleline);

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

            return new SourceAnalysisResult
            {
                Url = url,
                Title = ExtractTitle(html),
                MetaDescription = ExtractMetaDescription(html),
                LinkCount = LinkRegex.Matches(html).Count,
                ImageCount = ImageRegex.Matches(html).Count,
                HeadingCount = HeadingRegex.Matches(html).Count,
                WordCount = CountWords(html),
                CharacterCount = html.Length,
            };
        }

        private static string ExtractTitle(string html)
        {
            var match = TitleRegex.Match(html);
            return match.Success ? WebUtility.HtmlDecode(match.Groups[1].Value).Trim() : string.Empty;
        }

        private static string ExtractMetaDescription(string html)
        {
            var match = MetaDescriptionRegex.Match(html);
            return match.Success ? WebUtility.HtmlDecode(match.Groups[1].Value).Trim() : string.Empty;
        }

        private static int CountWords(string html)
        {
            var text = TagRegex.Replace(html, " ");
            text = WebUtility.HtmlDecode(text);
            return text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
