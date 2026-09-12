# Source analysis regression checks

Run with the .NET 8 SDK (no Windows UI runtime is needed):

```sh
dotnet run --project tests/SourceAnalysisRegression/SourceAnalysisRegression.csproj
```

This executable compiles the production service and checks metadata attribute order,
quotes and entities, actual element counts, exclusion of script/style/comment text,
body and fragment word counts, and the existing HTML source character count.

The application uses HtmlAgilityPack's UAP asset. These checks use its .NET asset;
a Windows UWP build is still required to validate application integration.

Manual UI check on Windows: analyze a page with a long description, shrink the
window, and verify that the final character-count line is reachable by vertical
scrolling and that the text wraps to the window width.
