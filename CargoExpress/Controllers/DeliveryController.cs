namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;

    public class DeliveryController : BaseController
    {
        private readonly IDeliveryService deliveryService;

        public DeliveryController(IDeliveryService _deliveryService)
		{
			this.deliveryService = _deliveryService;
        }

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(DeliveryCreateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

            try
            {
			    this.deliveryService.Create(model, User);
            }
            catch (InvalidOperationException ioe)
            {
				this.ModelState.AddModelError(nameof(model.PickedAt), ioe.Message);
            }

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			return RedirectToAction("All", "Cargo");
        }
    }
}
