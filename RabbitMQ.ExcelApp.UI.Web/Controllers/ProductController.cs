using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.ExcelApp.UI.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.ExcelApp.UI.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDBContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(AppDbContext appDBContext, UserManager<IdentityUser> userManager)
        {
            _appDBContext = appDBContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating
            };

            await _appDBContext.UserFiles.AddAsync(userFile);
            await _appDBContext.SaveChangesAsync();

            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));
        }

        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return View(await _appDBContext.UserFiles.Where(x => x.UserId == user.Id).ToListAsync());
        }
    }
}
