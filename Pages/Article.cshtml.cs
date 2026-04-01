using System.Text.RegularExpressions;
using System.Linq; // For Where/OrderBy
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiukaFotoRazor.Data;
using MiukaFotoRazor.Models;
using MiukaFotoRazor.Services;
using System.Text;

namespace MiukaFotoRazor.Pages
{
    /// <summary>
    /// Razor Page for rendering a single article and its paragraphs.
    /// Also extracts "fragment" anchors from encoded paragraph bodies.
    /// </summary>
    public class ArticleModel : PageModel
    {
        private readonly MiukaFotoDbContext _db;
        private readonly AppConfig _config;
        private readonly DocxExportService _docxService;

        public bool IsRoot { get; private set; }

        /// <summary>Image URL used when an image fails to load.</summary>
        public string FallbackImage => _config.FallbackImageUrl;

        /// <summary>Simple DTO for a decoded fragment anchor.</summary>
        public record FragmentPreview(string Id, string Text);

        /// <summary>List of fragments (anchors) extracted from paragraphs.</summary>
        public List<FragmentPreview> FragmentPreviews { get; set; } = new();

        // Pre-compiled regex: matches  ... 
        // Group 1 = id, Group 2 = inner text
        private static readonly Regex FragmentRegex = new(
            pattern: @"\|\|1a id=""([^""]+)\""\>(.*?)\|\|1/a>",
            options: RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        public ArticleModel(
            MiukaFotoDbContext db,
            IOptions<AppConfig> config,
            DocxExportService docxService
        )
        {
            _db = db;
            _config = config.Value;
            _docxService = docxService;
        }

        /// <summary>Route-bound ID of the article.</summary>
        [BindProperty(SupportsGet = true)]
        public long ID { get; set; }

        public ART_ARTICLE? Article { get; set; }
        public List<ART_PARAGRAPH> Paragraphs { get; set; } = new();

        /// <summary>
        /// Extracts fragment anchors from encoded paragraph HTML.
        /// Example encoded snippet:  Create the file 
        /// </summary>
        public static List<FragmentPreview> ExtractFragments(IEnumerable<ART_PARAGRAPH> paragraphs)
        {
            var fragments = new List<FragmentPreview>();

            foreach (var p in paragraphs)
            {
                if (string.IsNullOrEmpty(p.BODY)) continue;

                // Normalize input (remove newlines so regex can match across lines)
                var body = p.BODY.Replace("\r", "").Replace("\n", "");

                var matches = FragmentRegex.Matches(body);
                foreach (Match match in matches)
                {
                    var id = match.Groups[1].Value.Trim();
                    var text = match.Groups[2].Value.Trim();
                    if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(text))
                    {
                        fragments.Add(new FragmentPreview(id, text));
                    }
                }
            }

            return fragments;
        }

        /// <summary>Loads the article + paragraphs, then builds the fragment list.</summary>
        public async Task<IActionResult> OnGetAsync()
        {
            Article = await _db.ArtArticles.FindAsync(ID);
            if (Article == null) return NotFound();

            Paragraphs = await _db.ArtParagraphs.AsNoTracking()
                .Where(p => p.ARTICLE_ID == ID)
                .OrderBy(p => p.QUEUE)
                .ToListAsync();

            // Build fragment previews (mimicking Angular's fragment sidebar)
            FragmentPreviews = ExtractFragments(Paragraphs);
            var isRootStr = HttpContext.Session.GetString("IsRoot");
            IsRoot = string.Equals(isRootStr, "True", StringComparison.OrdinalIgnoreCase);
            return Page();
        }

        /// <summary>
        /// Decodes encoded HTML tokens into real angle brackets.
        /// Angular side used "<" for "<" and ">" for ">".
        /// </summary>
        public static string DecodeBody(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return string.Empty;

            // Decode your custom placeholders:
            //  - "<" -> "<"
            //  - ">" -> ">"
            // (Keep any other text unchanged)
            return raw.Replace("||" + "1", "<").Replace("1" + "||", ">");
        }

        public async Task<IActionResult> OnGetExportDocxAsync(int id)
        {
            // Fetch main article
            var main = await _db.ArtArticles
                .AsNoTracking()
                .Where(a => a.ID == id)
                .Select(a => new { a.IMAGE_URL, a.BODY })
                .FirstOrDefaultAsync();

            if (main == null)
                return NotFound();

            var mainBody = DecodeBody(main.BODY);

            // Fetch paragraphs
            var paragraphs = await _db.ArtParagraphs
                .AsNoTracking()
                .Where(p => p.ARTICLE_ID == id)
                .OrderBy(p => p.QUEUE)
                .Select(p => p.BODY)
                .ToListAsync();

            var paragraphBodies = paragraphs
                .Select(body => $"<p>{DecodeBody(body)}</p>");

            // Build final HTML (you can wrap in article container or add image if needed)
            var html = $@"
                <h1>Article {id}</h1>
                <div>{mainBody}</div>
                {string.Join(Environment.NewLine, paragraphBodies)}
            ";

            if (string.IsNullOrWhiteSpace(html))
                return NotFound();

            var fileBytes = await _docxService.ConvertHtmlToDocxAsync(html);

            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"article-{id}.docx");
        }

        private static string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html ?? string.Empty;

            // remove all <script>...</script> blocks (multiline, case-insensitive)
            return Regex.Replace(html, @"<script[\s\S]*?>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
        }

    }
}
