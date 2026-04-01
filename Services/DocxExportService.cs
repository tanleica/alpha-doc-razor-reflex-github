using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;

namespace MiukaFotoRazor.Services;

public class DocxExportService
{
    // Allow only safe HTML tags (extend as needed)
    private static readonly string[] SafeTags =
    {
        "p", "h1", "h2", "h3", "h4", "h5", "h6",
        "b", "i", "u", "strong", "em",
        "ul", "ol", "li",
        "table", "thead", "tbody", "tr", "td", "th",
        "img", "a", "blockquote", "br",
        "div", "span", "section", "article"
    };

    // Kill attributes starting with these prefixes (JS events etc.)
    private static readonly string[] KillTags = { "script", "iframe", "embed", "object", "style" };
    private static readonly string[] KillAttrPrefixes = { "on" }; // onclick, onmouseover, etc.
    private static readonly string[] AllowedAttrs = { "src", "href", "alt", "title", "class", "id" };

    private async Task<string> SanitizeAsync(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var context = BrowsingContext.New(Configuration.Default);
        var doc = await context.OpenAsync(req => req.Content(html));

        foreach (var el in doc.All.ToList())
        {
            // 1. Remove only *dangerous* tags
            if (KillTags.Contains(el.TagName.ToLower()))
            {
                Console.WriteLine($"Removed {el.TagName}");
                el.Remove();
                continue;
            }

            // 2. Strip unsafe attributes
            foreach (var attr in el.Attributes.ToList())
            {
                var name = attr.Name.ToLower();

                if (KillAttrPrefixes.Any(p => name.StartsWith(p)) ||
                    (!AllowedAttrs.Contains(name) && name.Contains("on")))
                {
                    Console.WriteLine($"Removed attribute {name} from <{el.TagName}>");
                    el.RemoveAttribute(attr.Name);
                }
            }
        }

        return doc.Body?.InnerHtml ?? string.Empty;
    }

    public async Task<byte[]> ConvertHtmlToDocxAsync(string html)
    {
        // Sanitize before converting
        var safeHtml = await SanitizeAsync(html);

        using var mem = new MemoryStream();

        using (var doc = WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body());

            var converter = new HtmlConverter(mainPart);
            var paragraphs = converter.Parse(safeHtml);

            foreach (var p in paragraphs)
                mainPart.Document.Body!.Append(p);

            mainPart.Document.Save();
        }

        return mem.ToArray();
    }
}

// using DocumentFormat.OpenXml.Packaging;
// using DocumentFormat.OpenXml.Wordprocessing;
// using HtmlToOpenXml;

// namespace MiukaFotoRazor.Services;

// public class DocxExportService
// {
//     public async Task<byte[]> ConvertHtmlToDocxAsync(string html)
//     {
//         using var mem = new MemoryStream();

//         using (var doc = WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
//         {
//             var mainPart = doc.AddMainDocumentPart();
//             mainPart.Document = new Document(new Body());

//             var converter = new HtmlConverter(mainPart);
//             var paragraphs = converter.Parse(html);

//             foreach (var p in paragraphs)
//                 mainPart.Document.Body!.Append(p);

//             mainPart.Document.Save();
//         }

//         return await Task.FromResult(mem.ToArray()); // keep async signature
//     }
// }
