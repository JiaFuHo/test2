using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using test2.Controllers;
using test2.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace test2.Areas.Frontend.Controllers
{
    [Area("Frontend")]
    public class HomeController : UserController
    {
        #region field
        public const string sk1 = "query1";
        public const string sk2 = "type1";
        public const string sk3 = "query2";
        public const string sk4 = "year1";
        public const string sk5 = "year2";
        public const string sk6 = "lang";
        public const string sk7 = "type2";
        public const string sk8 = "status";

        private readonly Test2Context _context;
        #endregion

        #region constructor
        public HomeController(Test2Context context) { _context = context; }
        #endregion

        #region action
        public IActionResult Index() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Query(string query1, string type1, string query2, string year1, string year2, string lang, string type2, string status)
        {
            if (string.IsNullOrEmpty(query1)) { query1 = string.Empty; }
            if (string.IsNullOrEmpty(type1)) { type1 = string.Empty; }
            if (string.IsNullOrEmpty(query2)) { query2 = string.Empty; }
            if (string.IsNullOrEmpty(year1)) { year1 = string.Empty; }
            if (string.IsNullOrEmpty(year2)) { year2 = string.Empty; }
            if (string.IsNullOrEmpty(lang)) { lang = string.Empty; }
            if (string.IsNullOrEmpty(type2)) { type2 = string.Empty; }
            if (string.IsNullOrEmpty(status)) { status = string.Empty; }

            HttpContext.Session.SetString(sk1, query1);
            HttpContext.Session.SetString(sk2, type1);
            HttpContext.Session.SetString(sk3, query2);
            HttpContext.Session.SetString(sk4, year1);
            HttpContext.Session.SetString(sk5, year2);
            HttpContext.Session.SetString(sk6, lang);
            HttpContext.Session.SetString(sk7, type2);
            HttpContext.Session.SetString(sk8, status);

            IQueryable<Collection> query = _context.Collections.Include(x => x.Author).Include(x => x.Language).Include(x => x.Type).Include(x => x.Books);

            if (!string.IsNullOrEmpty(query1)) { query = query.Where(i => i.Title.Contains(query1) || i.Author.Author1.Contains(query1) || i.Publisher.Contains(query1) || i.Isbn.Contains(query1)); }
            else
            {
                bool x = int.TryParse(year1, out int yearX);
                bool y = int.TryParse(year2, out int yearY);

                if (!string.IsNullOrEmpty(query2))
                {
                    switch (type1)
                    {
                        case "title":
                            query = query.Where(i => i.Title.Contains(query2)); break;
                        case "author":
                            query = query.Where(i => i.Author.Author1.Contains(query2)); break;
                        case "publisher":
                            query = query.Where(i => i.Publisher.Contains(query2)); break;
                        case "isbn":
                            query = query.Where(i => i.Isbn.Contains(query2)); break;
                        default: break;
                    }
                }
                if (x) { query = query.Where(i => i.PublishDate.Year >= yearX); }
                if (y) { query = query.Where(i => i.PublishDate.Year <= yearY); }
                if (!string.IsNullOrEmpty(lang)) { query = query.Where(i => i.Language.LanguageId == Convert.ToInt16(lang)); }
                if (!string.IsNullOrEmpty(type2)) { query = query.Where(i => i.Type.TypeId == Convert.ToInt16(type2)); }
            }

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Collection(string type, string author, string publisher, string lang, string year1, string year2)
        {
            IQueryable<Collection> collection = _context.Collections.Include(x => x.Author).Include(x => x.Language).Include(x => x.Type);

            bool x = int.TryParse(year1, out int yearX);
            bool y = int.TryParse(year2, out int yearY);

            if (!string.IsNullOrEmpty(type)) { collection = collection.Where(i => i.Type.Type1.Contains(type)); }
            if (!string.IsNullOrEmpty(author)) { collection = collection.Where(i => i.Author.Author1.Contains(author)); }
            if (!string.IsNullOrEmpty(publisher)) { collection = collection.Where(i => i.Publisher.Contains(publisher)); }
            if (!string.IsNullOrEmpty(lang)) { collection = collection.Where(i => i.Language.Language1.Contains(lang)); }
            if (x) { collection = collection.Where(i => i.PublishDate.Year >= yearX); }
            if (y) { collection = collection.Where(i => i.PublishDate.Year <= yearY); }

            return View(await collection.ToListAsync());
        }

        public async Task<IActionResult> Client()
        {
            var userNameX = Convert.ToString(ViewData["UserName"]);

            IQueryable<Client> client = _context.Clients.Include(x => x.Borrows).ThenInclude(y => y.BorrowStatus)
                                                                                     .Include(w => w.Borrows).ThenInclude(x => x.Book).ThenInclude(y => y.Collection).ThenInclude(z => z.Author)
                                                                                     .Include(x => x.Borrows).ThenInclude(y => y.Histories)
                                                                                     .Include(x => x.Favorites).ThenInclude(y => y.Collection).ThenInclude(z => z.Author)
                                                                                     .Include(x => x.Notifications)
                                                                                     .Where(x => x.CName.Contains(userNameX!));

            return View(await client.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(string idInput1, string passwordInput3)
        {
            int.TryParse(idInput1, out int cId);

            var userId = await _context.Clients.FindAsync(cId);

            userId!.CPassword = Argon2.Hash(passwordInput3);

            await _context.SaveChangesAsync();

            return RedirectToAction("Client");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhone(string idInput2, string phoneInput2)
        {
            int.TryParse(idInput2, out int cId);

            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC pr_updatePhone {cId}, {phoneInput2}");

            var userPhoneX = await _context.Clients.Include(x => x.Borrows).ThenInclude(y => y.BorrowStatus)
                                                                              .Include(w => w.Borrows).ThenInclude(x => x.Book).ThenInclude(y => y.Collection).ThenInclude(z => z.Author)
                                                                              .Include(x => x.Borrows).ThenInclude(y => y.Histories)
                                                                              .Include(x => x.Favorites).ThenInclude(y => y.Collection).ThenInclude(z => z.Author)
                                                                              .Include(x => x.Notifications)
                                                                              .AsNoTracking().FirstOrDefaultAsync(i => i.CId == cId);

            return RedirectToAction("Client", new List<Client> { userPhoneX! });
        }
        #endregion
    }
}