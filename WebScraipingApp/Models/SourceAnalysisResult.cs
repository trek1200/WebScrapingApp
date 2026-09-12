namespace WebScraipingApp.Models
{
    public class SourceAnalysisResult
    {
        public string Url { get; set; }

        public string Title { get; set; }

        public string MetaDescription { get; set; }

        public int LinkCount { get; set; }

        public int ImageCount { get; set; }

        public int HeadingCount { get; set; }

        public int WordCount { get; set; }

        public int CharacterCount { get; set; }
    }
}
