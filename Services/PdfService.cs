using DinkToPdf;
using DinkToPdf.Contracts;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Models.ViewModels;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class PdfService : IPdfService
{
    private readonly IRazorViewRenderer _viewRenderer;
    private readonly IConverter _pdfConverter;

    public PdfService(IRazorViewRenderer viewRenderer)
    {
        _viewRenderer = viewRenderer;
        _pdfConverter = new SynchronizedConverter(new PdfTools());
    }

    public async Task<byte[]> GenerateProductPdfAsync(List<ProductDTO> products)
    {
        var model = new ProductListViewModel { Products = products, GeneratedDate = DateTime.Now };
        // Renderizar la vista Razor a HTML
        string htmlContent = await _viewRenderer.RenderViewToStringAsync("Pdfs/products", model);

        // Configurar opciones del PDF
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings =
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings
                {
                    Top = 10,
                    Bottom = 10,
                    Left = 10,
                    Right = 10,
                },
            },
            Objects =
            {
                new ObjectSettings
                {
                    HtmlContent = htmlContent,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    PagesCount = true,
                    HeaderSettings =
                    {
                        FontSize = 9,
                        Right = "Página [page] de [toPage]",
                        Line = true,
                    },
                    FooterSettings =
                    {
                        FontSize = 9,
                        Center = $"TecnoCredito © {DateTime.Now.Year}",
                        Line = true,
                    },
                },
            },
        };

        // Convertir HTML a PDF
        byte[] pdf = _pdfConverter.Convert(doc);
        return pdf;
    }
}
