using System.Threading.Tasks;
using WebScraipingApp.Models;

namespace WebScraipingApp.Services
{
    public interface ISourceAnalysisService
    {
        Task<SourceAnalysisResult> AnalyzeAsync(string url);
    }
}
