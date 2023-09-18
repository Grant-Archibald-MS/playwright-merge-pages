using Microsoft.Playwright;
using org.pdfclown.files;
using System.Configuration;

using var playwright = await Playwright.CreateAsync();
var context = string.Format(@"C:\Users\{0}\AppData\Local\Microsoft\Edge\User Data\Default", Environment.UserName);
await using var browser = await playwright.Chromium.LaunchPersistentContextAsync(context, new BrowserTypeLaunchPersistentContextOptions {
    Channel = "msedge", 
    Headless = false
});

var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
var homePage = ConfigurationManager.AppSettings["Home"];
var pages = ConfigurationManager.AppSettings["Pages"];
var coverPage = ConfigurationManager.AppSettings["Cover"];

var page = await browser.NewPageAsync();
await page.GotoAsync(string.Format(baseUrl,homePage));

// await page.PauseAsync();

var pdf = await page.PdfAsync(new PagePdfOptions() { DisplayHeaderFooter = false });
System.IO.File.WriteAllBytes( "00-" + homePage + ".pdf", pdf);

int counter = 0;
foreach ( var pageName in pages.Split(',')) {
    page = await browser.NewPageAsync();
    await page.GotoAsync(string.Format(baseUrl,pageName));
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    
    var match = page.Locator("img");
    var images = await match.AllAsync();
    if ( images.Count() > 0 ) {
        foreach ( var image in images ) {
            var src = await image.GetAttributeAsync("src");
            if ( !string.IsNullOrEmpty(src) && src.StartsWith("media")) {
                await image.EvaluateAsync(string.Format("node => node.src = \"{0}\"", string.Format(baseUrl, src)));
            }
        }
    }
    counter++;

    pdf = await page.PdfAsync(new PagePdfOptions() { DisplayHeaderFooter = false, PreferCSSPageSize = true });
    System.IO.File.WriteAllBytes( (100 + counter).ToString().Substring(1) + "-" + pageName + ".pdf", pdf);
    await page.CloseAsync();
}
MergePdf(".", "merged.pdf", coverPage);

static void MergePdf(string srcPath, string destFile, string cover)
{
    var list = Directory.GetFiles(Path.GetFullPath(srcPath));

    if (string.IsNullOrWhiteSpace(srcPath) || string.IsNullOrWhiteSpace(destFile) || list.Length <= 1)
        return;

    var files = list
        .Where(f => {
                int result;
                var name = Path.GetFileName(f);
                return int.TryParse(name.Substring(0,1), out result);
            })
        .OrderBy(f => f)
        .Select(System.IO.File.ReadAllBytes)
        .ToList();

    if ( files.Count == 0 ) {
        Console.WriteLine("No matching files found");
        return;
    }

    using (var dest = new org.pdfclown.files.File(new org.pdfclown.bytes.Buffer(System.IO.File.ReadAllBytes(cover))))
    {
        var document = dest.Document;
        var builder = new org.pdfclown.tools.PageManager(document);
        foreach (var file in files)
        {
            using (var src = new org.pdfclown.files.File(new org.pdfclown.bytes.Buffer(file)))
            { 
                builder.Add(src.Document);
            }
        }

        dest.Save(destFile, SerializationModeEnum.Incremental);
    }
}