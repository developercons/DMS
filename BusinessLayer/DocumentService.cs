using DataLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BusinessLayer
{
    public class DocumentService
    {
        private readonly DMSContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        public DocumentService(DMSContext db)
        {
            _context = db;
        }

        public DocumentService(DMSContext db, IHostingEnvironment appEnvironment)
        {
            _context = db;
            _appEnvironment = appEnvironment;
        }

        public async Task<IQueryable<DocumentViewModel>> getAllDocuments(string email)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            var items = from x in _context.Documents
                    where x.UsersUserId == user.UserId
                    select new DocumentViewModel
                    {
                        DocumentId = x.DocumentId,
                        DocumentPath = x.DocumentPath,
                        DocumentName = x.DocumentName,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category.CategoryName
                    };
            return items.AsQueryable();
        }
        public bool documentUpload(IFormFile file, string path,Document document,string email)
        {
            var user = _context.Users.Where(x => x.UserEmail == email).FirstOrDefault();
            string path_Root = path;
            string filePath = "\\Documents\\uid-" + user.UserId + file.GetFilename();
            string extention= Path.GetExtension(file.FileName);
            if (extention == ".pdf" || extention == ".PDF" || extention == ".doc" || extention == ".DOC" || extention == ".docx" || extention == ".DOCX")
            {
                try
                {
                    using (var stream = new FileStream(path_Root+filePath, FileMode.Create))
                    {
                        Document item = new Document();
                        item.DocumentPath = filePath;
                        item.DocumentName = "uid-" + user.UserId + file.FileName;
                        item.DocumentTags = file.FileName;//default tags given same as filename will be replaced later
                        item.CategoryId = document.CategoryId;
                        item.UsersUserId = user.UserId;
                        _context.Add(item);
                        _context.SaveChanges();
                        file.CopyTo(stream);


                    }
                }
                catch (Exception ex)
                {
                    var exp = ex;
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public string getPath(int id)
        {
            var item = _context.Documents.Where(x => x.DocumentId == id).FirstOrDefault();
            return item.DocumentPath;
        }

        public string getName(int id)
        {
            var item = _context.Documents.Where(x => x.DocumentId == id).FirstOrDefault();
            return item.DocumentName;
        }

    }
}
