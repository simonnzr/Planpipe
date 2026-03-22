using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using Planpipe.Core.Interfaces;

namespace Planpipe.Infrastructure.Services;

public class PdfTextExtractor : IPdfTextExtractor
{
    public async Task<string> ExtractTextAsync(Stream pdfStream)
    {
        return await Task.Run(() =>
        {
            var sb = new StringBuilder();
            
            using (var document = PdfDocument.Open(pdfStream))
            {
                foreach (Page page in document.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }
            
            return sb.ToString();
        });
    }
}
