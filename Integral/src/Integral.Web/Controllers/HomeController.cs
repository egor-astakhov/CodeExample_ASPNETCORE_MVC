using Integral.Application.Landing.Queries;
using Integral.Application.Notifications.Commands.SendContactUsNotification;
using Integral.Application.Storage.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Integral.Web.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await Mediator.Send(new GetLandingViewModelQuery());

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ProductAttachment(int id)
        {
            var dto = await Mediator.Send(new GetProductAttachmentFileQuery(id));
            if (dto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return File(dto.Stream, "application/pdf", dto.Name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(SendContactUsNotificationCommand request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(ErrorController.Index), "Error", new
                {
                    code = (int)HttpStatusCode.BadRequest
                });
            }

            await Mediator.Send(request);

            return View("RequestSuccessfullySent");
        }
    }
}

