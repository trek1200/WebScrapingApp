using System;
using WebScraipingApp.Services;

internal static class Program
{
    private static void Equal<T>(T expected, T actual, string name)
    {
        if (!object.Equals(expected, actual))
            throw new Exception(name + ": expected " + expected + ", got " + actual);
    }

    private static void Main()
    {
        const string url = "https://example.com";
        var reversed = SourceAnalysisService.AnalyzeHtml(url,
            "<meta content=\"Page summary\" name=\"description\">");
        Equal("Page summary", reversed.MetaDescription, "Attribute order");

        var quoted = SourceAnalysisService.AnalyzeHtml(url,
            "<META NAME = 'DESCRIPTION' CONTENT = \"It's a test &amp; example\">");
        Equal("It's a test & example", quoted.MetaDescription, "Quotes, casing and entities");

        const string html = "<html><head><title>Example &amp; Test</title>" +
            "<script>const template = '<a href=\"/\">fake</a><img src=\"x\"><h1>fake</h1>';</script>" +
            "<style>.example { content: '<a>fake</a>'; }</style></head>" +
            "<body><!-- <a>fake</a><img><h1>fake</h1> -->" +
            "<h2>Hello</h2><a href=\"/\">world</a><img src=\"real.png\"></body></html>";
        var result = SourceAnalysisService.AnalyzeHtml(url, html);
        Equal("Example & Test", result.Title, "Title decoding");
        Equal(1, result.LinkCount, "Real links only");
        Equal(1, result.ImageCount, "Real images only");
        Equal(1, result.HeadingCount, "Real headings only");
        Equal(2, result.WordCount, "Body text excludes scripts, styles, comments and title");
        Equal(html.Length, result.CharacterCount, "Source character count");
        Equal(url, result.Url, "Source URL");

        var fragment = SourceAnalysisService.AnalyzeHtml(url,
            "<p>Hello&nbsp;world</p><script>var hidden = 1;</script><style>p { color: red; }</style>");
        Equal(2, fragment.WordCount, "Fragment and nonbreaking space");
        Equal(string.Empty, fragment.Title, "Missing title");
        Equal(string.Empty, fragment.MetaDescription, "Missing description");

        var empty = SourceAnalysisService.AnalyzeHtml(url, string.Empty);
        Equal(0, empty.WordCount, "Empty text");
        Equal(0, empty.LinkCount, "Empty links");
        Console.WriteLine("All source analysis regression checks passed.");
    }
}
