namespace CargoExpress.Controllers
{
	using CargoExpress.Core.Contracts;
	using CargoExpress.Core.Models;
	using CargoExpress.Infrastructure.Data.Models;
	using CargoExpress.Infrastructure.Data.Repositories;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using System.Security.Principal;

	public class DeliveryController : BaseController
    {
        private readonly IApplicationDbRepository repo;
        private readonly IDeliveryService deliveryService;


        public DeliveryController(IApplicationDbRepository _repo, IDeliveryService _deliveryService)
		{
            this.repo = _repo;
			this.deliveryService = _deliveryService;
        }

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(DeliveryCreateViewModel model)
		{
            /*
            var isExists = this.repo.All<Delivery>().Any(d => d.DeliveryRef == model.DeliveryRef);

            if (isExists)
            {
                this.ModelState.AddModelError(nameof(model.DeliveryRef), "The delivery has already been added!");
            }
            */
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            this.deliveryService.Create(model, User);

            return RedirectToAction("All", "Cargo");
            //return View();
        }
    }
}
