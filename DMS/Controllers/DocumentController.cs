using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataLayer;
using BusinessLayer;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace DMS.Controllers
{
    [Authorize(Roles= "User,Admin")]
    public class DocumentController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly DocumentService _doc;
        private CategoryService _cats;
        public DocumentController(
            IHostingEnvironment appEnvironment,
            DMSContext context )
        {
            _appEnvironment = appEnvironment;
            _doc =new DocumentService(context);
            _cats = new CategoryService(context);
        }

        public async Task<IActionResult> Index(int page=1)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var documentList =await _doc.getAllDocuments(email);
            int pageSize = 3;
            return View(await PaginatedList<DocumentViewModel>.CreateAsync(documentList.AsNoTracking(), page, pageSize));
        }

        public IActionResult Create()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            ViewBag.categories = _cats.GetAll(email);
            return View();
          
        }
   
        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file,Document document)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            ViewBag.categories = _cats.GetAll(email);
            string extention = Path.GetExtension(file.FileName);
            if (extention == ".pdf" || extention == ".PDF" || extention == ".doc" || extention == ".DOC" || extention == ".docx" || extention == ".DOCX")
            {
                if (file == null || file.Length == 0 || file.Length > 4000000)
                {
                    ViewBag.error = "File either empty or max size exeeds.";
                    return View();
                }
                string path_Root = _appEnvironment.WebRootPath;
                if (_doc.documentUpload(file, path_Root, document, email))
                {

                    ViewBag.success = "Successfully document uploaded.";
                }
            }
            else
            {
                ViewBag.error = ".pdf / .doc files are allowed only";
                return View();
            }
            return View();
        }

        public async Task<FileResult> Download(int id)
        {
            string filePath = _doc.getPath(id);
            string fileName = _doc.getName(id);
            var path = Path.Combine(
                                  Directory.GetCurrentDirectory(),
                                       "wwwroot\\Documents\\", fileName);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}