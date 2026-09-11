using System;
using Prism.Commands;
using Prism.Windows.Mvvm;
using WebScraipingApp.Models;
using WebScraipingApp.Services;

namespace WebScraipingApp.ViewModels
{
    class MainPageViewModel : ViewModelBase
    {

#region "Hello World"


        private string input;

        public string Input {
            get { return input; }
            set { SetProperty(ref input, value); }
        }

        private string output;

        public string Output
        {
            get { return output; }
            set { SetProperty(ref output, value); }
        }

        public DelegateCommand CreateOutputCommand { get; }

        #endregion

#region "Source Content Analysis"

        private readonly ISourceAnalysisService sourceAnalysisService = new SourceAnalysisService();

        private string sourceUrl;

        public string SourceUrl
        {
            get { return sourceUrl; }
            set { SetProperty(ref sourceUrl, value); }
        }

        private bool isAnalyzing;

        public bool IsAnalyzing
        {
            get { return isAnalyzing; }
            set { SetProperty(ref isAnalyzing, value); }
        }

        private string analysisStatus;

        public string AnalysisStatus
        {
            get { return analysisStatus; }
            set { SetProperty(ref analysisStatus, value); }
        }

        private string analysisSummary;

        public string AnalysisSummary
        {
            get { return analysisSummary; }
            set { SetProperty(ref analysisSummary, value); }
        }

        public DelegateCommand AnalyzeSourceCommand { get; }

        public MainPageViewModel()
        {
            CreateOutputCommand = new DelegateCommand(() =>
            {
                Output = $"{Input}が入力されました。";

            }, () => !string.IsNullOrWhiteSpace(Input)).ObservesProperty(() => Input);

            AnalyzeSourceCommand = new DelegateCommand(async () =>
            {
                IsAnalyzing = true;
                AnalysisStatus = "解析中です...";
                AnalysisSummary = string.Empty;

                try
                {
                    var result = await sourceAnalysisService.AnalyzeAsync(SourceUrl);
                    AnalysisSummary =
                        $"タイトル: {result.Title}\n" +
                        $"メタ説明: {result.MetaDescription}\n" +
                        $"リンク数: {result.LinkCount}\n" +
                        $"画像数: {result.ImageCount}\n" +
                        $"見出し数: {result.HeadingCount}\n" +
                        $"単語数: {result.WordCount}\n" +
                        $"文字数: {result.CharacterCount}";
                    AnalysisStatus = "解析が完了しました。";
                }
                catch (Exception ex)
                {
                    AnalysisStatus = $"解析に失敗しました: {ex.Message}";
                }
                finally
                {
                    IsAnalyzing = false;
                }
            }, () => !IsAnalyzing && !string.IsNullOrWhiteSpace(SourceUrl))
                .ObservesProperty(() => SourceUrl)
                .ObservesProperty(() => IsAnalyzing);
        }

#endregion

    }
}
