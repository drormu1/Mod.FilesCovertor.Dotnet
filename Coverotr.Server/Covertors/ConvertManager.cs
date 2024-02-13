using System.IO;
using System.Text;
using Aspose.Pdf;
using Aspose.Pdf.Devices;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging;
using Mod.FilesCovertor.Server;
using System;
using Aspose.Pdf.Facades;
using Aspose.Words.Saving;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Aspose.Pdf.Annotations;
using Microsoft.Extensions.Options;
using Aspose.Pdf.Operators;
using Aspose.Words;
using Aspose.Words.Layout;
using Aspose.Slides;
using System.IO.Pipes;
using System.Reflection;
using System.Drawing.Imaging;



namespace Mod.FilesCoverotor.Server.Covertors
{
    public class ConvertManager
    {
        public string ConvertPdfToImage(ConvertRequest convertRequest)
        { 
            byte[] buffer = Convert.FromBase64String(convertRequest.ContentAs64);
            string base64String = "";
         
            using (MemoryStream pdfStream = new MemoryStream(buffer))
            {
                Aspose.Pdf.Document document = new Aspose.Pdf.Document(pdfStream);
                Aspose.Pdf.Document onePagedocument = new Aspose.Pdf.Document();
                onePagedocument.Pages.Add( document.Pages[0]);                
                using (MemoryStream imageStream = new MemoryStream())
                {                    
                    PdfConverter converter = new PdfConverter();
                    converter.BindPdf(onePagedocument);
                    converter.DoConvert();
                    converter.GetNextImage(imageStream, PageSize.A4);
                    base64String = Convert.ToBase64String(imageStream.ToArray());
                }
            }
            return base64String;
        }

        public  string ConvertDocxToSvg(ConvertRequest convertRequest)
        {
            
             byte[] buffer = Convert.FromBase64String(convertRequest.ContentAs64);
             string base64String = "";
            // Step 3: Load PDF document using Aspose.PDF
            using (MemoryStream stream = new MemoryStream(buffer))
            {

                Aspose.Words.Document document = new Aspose.Words.Document(stream);
                document = document.ExtractPages(0, 1);
                MemoryStream outStream = new MemoryStream();
                Aspose.Words.Saving.SvgSaveOptions option = new Aspose.Words.Saving.SvgSaveOptions();
                option.SaveFormat = Aspose.Words.SaveFormat.Svg;                
                option.ExportEmbeddedImages = true;
                document.Save(outStream, option);
                base64String = Convert.ToBase64String(outStream.ToArray());                
            }
            return base64String;
        }

        public string ConvertPptToPdf(ConvertRequest convertRequest)
        {
            byte[] buffer = Convert.FromBase64String(convertRequest.ContentAs64);
            string base64String = "";
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                Presentation presentation = new Presentation(stream);
                using (MemoryStream pptStream = new MemoryStream())
                {
                    presentation.Save(pptStream,Aspose.Slides.Export.SaveFormat.Pdf);
                    base64String = Convert.ToBase64String(pptStream.ToArray());
                }                
            }         
            return base64String;
        }

        public string ConvertPptToPng(ConvertRequest convertRequest)
        {
            byte[] buffer = Convert.FromBase64String(convertRequest.ContentAs64);
            string base64String = "";
            System.Drawing.Size size = new System.Drawing.Size(960, 720);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                Presentation presentation = new Presentation(stream);
                using (MemoryStream imageStream = new MemoryStream())
                {
                    ISlide slide = presentation.Slides[0];
                    //slide.WriteAsSvg(svgStream);
                    slide.GetThumbnail(size).Save(imageStream, ImageFormat.Png);
                    base64String = Convert.ToBase64String(imageStream.ToArray());
                }
            }
            return base64String;
        }
    }
}
