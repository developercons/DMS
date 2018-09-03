using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using MimeKit;

namespace BusinessLayer
{
    public class UserService
    {
        private readonly DMSContext _context;
        public UserService(DMSContext context)
        {
            _context = context;
        }
        public List<User> GetAll()
        {
            var _users = _context.Users.ToList();
            return _users;
        }
        public bool Create(User user)
        {
            bool status;
            User item = new User();
            item.UserName = user.UserName;
            item.UserEmail = user.UserEmail;
            item.password = user.password;
            item.UserRole = user.UserRole;
            try
            {
                _context.Users.Add(item);
                _context.SaveChanges();
                SendMail(user.UserName, user.UserEmail,user.password);
                status = true;
            }
            catch (Exception ex)
            {
                var exp = ex;
                status = false;
            }
            return status;
        }
        public bool Delete(int id)
        {
            bool status;
            var item = _context.Users.Find(id);
            try
            {
                _context.Users.Remove(item);
                _context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                var exp  = ex;
                status = false;
            }
            return status;
        }
        public bool SendMail(string name,string email,string password)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            MailAddress from = new MailAddress("replace_your_gmail_id_here@gmail.com", "Admin-DMS");
            MailAddress to = new MailAddress(email, name);
            MailMessage message = new MailMessage(from, to);
            message.Body ="Hello,"+name +"! Please use these credentials to sign in DMS Software. Email="+email+" and password= "+password+"  .Thank you!";
            message.Subject = "DMS- USER LOGIN DETAILS";
            NetworkCredential myCreds = new NetworkCredential("nayeem.azad.cse@gmail.com", "replace_your_gmail_password_here", "");
            client.Credentials = myCreds;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                var exp = ex;
            }      
            return true;
         }
    }
}
