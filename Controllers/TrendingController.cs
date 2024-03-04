using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using DataMining_MVC.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using Telegram.Bot;
using System.Net.Http;
using System.Text;
using iText.Layout.Element;
using Microsoft.AspNetCore.Components;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataMining_MVC.Controllers
{
    public class TrendingController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7111/api/");

        private readonly HttpClient _client;


        public TrendingController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<TrendingPostViewModel> postList = new List<TrendingPostViewModel>();
            HttpResponseMessage response = _client.GetAsync(baseAddress + "Memes/getTopTrendingMeme").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                postList = JsonConvert.DeserializeObject<List<TrendingPostViewModel>>(data);
                return View(postList);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }

        [HttpGet]
        public IActionResult GetTopTwentyTrendingPosts()
        {
            List<PostModel> postList = GetTopTwentyMemes();
            if (postList != null)
            {
                return View(postList);
            }
            else
            {
                string errorMessage = "An error occurred. Please try again later.";

                ViewData["ErrorMessage"] = errorMessage;
                return View("Error");

            }

        }

        public  List<PostModel> GetTopTwentyMemes()
        {
            List<PostModel> postList = new List<PostModel>();
            HttpResponseMessage response = _client.GetAsync(baseAddress + "Memes/GetTopTwentyMemes").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                postList = JsonConvert.DeserializeObject<List<PostModel>>(data);
                return postList;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        public IActionResult GenerateReport()
        {
            List<PostModel> postList = GetTopTwentyMemes();

            if (postList == null)
            {
                return Content("No data available to generate report.");
            }
            else
            {
               byte[] Document= CreateDocument(postList);
              
                return View(Document);

            }
        }

        [HttpPost]
        public async Task<IActionResult> SendTelegramPDF()
        {
            long chatId = 5559204732;
            string token = "6303059270:AAG39WQJfEfKtwVoaIWkv-vywsXyfop6CwY";

            List<PostModel> postList = GetTopTwentyMemes();
            
            if (postList == null)
            {
                string errorMessage = "An error occurred. Please try again later.";

                ViewData["ErrorMessage"] = errorMessage;
                return View("Error");
            }
            else
            {

                byte[] Document = CreateDocument(postList);

                
                string apiUrl = $"https://api.telegram.org/bot{token}/sendDocument";
                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

               
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(chatId.ToString()), "chat_id");
                formData.Add(new ByteArrayContent(Document), "document", "document.pdf");
                request.Content = formData;

                using (HttpClient client = new HttpClient())
                {
                    
                    var response1 =await client.SendAsync(request);


                    if (response1.IsSuccessStatusCode)
                    {
                        var successMessage = new { message = "Successfully sent to Telegram bot" };
                        return Json(successMessage);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Error", new { parameterName = "Document cannot be sent to HepMil Telegrambot" });
                    }
                }
            }

        }

       
        public static byte[] CreateDocument(List<PostModel> postList)
        {
            MemoryStream ms = new MemoryStream();
            Document document = new Document(iTextSharp.text.PageSize.LETTER, 0, 0, 0, 0);
            PdfWriter pw = PdfWriter.GetInstance(document, ms);
            document.Open();

            iTextSharp.text.Paragraph Heading = new iTextSharp.text.Paragraph("HepMil Media", FontFactory.GetFont("Times New Roman", 25, Font.BOLD, new BaseColor(102, 0, 102)));
            Heading.Alignment = Element.ALIGN_CENTER;
            document.Add(Heading);

            iTextSharp.text.Paragraph trendingParagraph = new iTextSharp.text.Paragraph("Trending Top Twenty Reddit Memes", FontFactory.GetFont("Times New Roman", 20, Font.BOLD, new BaseColor(102, 0, 102)));
            trendingParagraph.Alignment = Element.ALIGN_CENTER;
            document.Add(trendingParagraph);

            document.Add(new iTextSharp.text.Paragraph(" "));

            document.Add(new iTextSharp.text.Paragraph(" "));

            PdfPTable table = new PdfPTable(6);

            table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
            table.DefaultCell.Padding = 5;

            
            table.AddCell(CreatCell("No"));
            table.AddCell(CreatCell("Post Name"));
            table.AddCell(CreatCell("Total Votes"));
            table.AddCell(CreatCell("Total Comments"));
            table.AddCell(CreatCell("URL"));
            table.AddCell(CreatCell("Timestamp"));


      
            foreach (var post in postList)
            {
                table.AddCell(post.id.ToString());
                table.AddCell(post.Title);
                table.AddCell(post.Ups.ToString());
                table.AddCell(post.Commnets.ToString());
                table.AddCell(post.Url.ToString());
                table.AddCell(post.timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                
            }


            document.Add(table);

            document.Close();
            byte[] bytesStream = ms.ToArray();
            return bytesStream;
        }

        public static PdfPCell CreatCell(String text)
        {
            var cell = new PdfPCell(new Phrase(text));
            cell.BackgroundColor = new BaseColor(102, 0, 102);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            cell.Phrase = new Phrase(text, FontFactory.GetFont("Times New Roman ", 12, Font.NORMAL, BaseColor.WHITE));
            return cell;
        }
    }
    }




