using Integral.Application.ApplicationSettings.Data;
using Integral.Application.ApplicationSettings.Queries;
using Integral.Application.Common.Persistence;
using Integral.Application.Common.Security;
using Integral.Application.Products.Commands;
using Integral.Application.Products.Data;
using Integral.Application.Products.Queries;
using Integral.Application.Storage.Commands;
using Integral.Application.Storage.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Integral.Web.Controllers
{
    [Authorize(Policy = Policy.OnlyForAdmins)]
    public class AdminController : BaseController
    {
        private readonly IDeferredDbContext _dbContext;

        public AdminController(IDeferredDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CommonSettings()
        {
            var model = await Mediator.Send(new GetCommonSettingsQuery());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommonSettings(CommonSettingsViewModel request)
        {
            if (ModelState.IsValid)
            {
                await Mediator.Send(request);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LandingCarousel()
        {
            var model = await Mediator.Send(new GetLandingCarouselSettingsQuery());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LandingCarousel(LandingCarouselSettingsViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            await Mediator.Send(request);

            return RedirectToAction(nameof(LandingCarousel));
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var model = await Mediator.Send(new GetProductListQuery());

            return View(model);
        }

        [HttpGet]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> EditProduct(int? id)
        {
            var model = await Mediator.Send(new GetProductForEditingQuery(id));
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(EditProductViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            await Mediator.Send(request);

            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await Mediator.Send(new DeleteProductCommand(id));

            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EmailServiceSettings()
        {
            var model = await Mediator.Send(new GetEmailServiceSettingsQuery());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailServiceSettings(EmailServiceSettingsViewModel request)
        {
            if (ModelState.IsValid)
            {
                await Mediator.Send(request);
            }

            return View();
        }

        [HttpGet]
        public IActionResult DeferredChanges()
        {
            if (_dbContext.DeferredChanges.Any())
            {
                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersistDeferredChanges()
        {
            await _dbContext.PersistChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DiscardDeferredChanges()
        {
            _dbContext.DiscardChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Backups()
        {
            var model = await Mediator.Send(new GetBackupsQuery());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBackup()
        {
            await Mediator.Send(new CreateBackupCommand());

            return RedirectToAction(nameof(Backups));
        }

        [HttpGet]
        public async Task<IActionResult> GetBackup(string name)
        {
            var dto = await Mediator.Send(new GetBackupFileQuery(name));
            if (dto == null)
            {
                return RedirectToAction(nameof(Backups));
            }

            return File(dto.Stream, "application/zip", dto.Name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBackup(string name)
        {
            await Mediator.Send(new DeleteBackupCommand(name));

            return RedirectToAction(nameof(Backups));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyBackup(ApplyBackupCommand command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Backups));
            }

            await Mediator.Send(command);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult FAQ()
        {
            return View();
        }
    }
}