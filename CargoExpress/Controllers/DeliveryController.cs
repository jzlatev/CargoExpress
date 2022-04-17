namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    public class DeliveryController : BaseController
    {
        private readonly IDeliveryService deliveryService;

        public DeliveryController(IDeliveryService _deliveryService)
		{
			this.deliveryService = _deliveryService;
			this.EntityName = "Delivery";
        }

		public IActionResult Create()
		{
			var model = new DeliveryCreateViewModel();
			deliveryService.PopulateWarehouse(model);
			deliveryService.PopulateTruck(model);

			return View(model);
		}

		[HttpPost]
		public IActionResult Create(DeliveryCreateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				deliveryService.PopulateWarehouse(model);
				deliveryService.PopulateTruck(model);

				return View(model);
			}

            try
            {
			    this.deliveryService.Create(model, User);
            }
            catch (FormException e)
            {
				this.ModelState.AddModelError(e.InputName, e.ErrorMessage);
            }

			if (!ModelState.IsValid)
			{
				deliveryService.PopulateWarehouse(model);
				deliveryService.PopulateTruck(model);

				return View(model);
			}

			return RedirectToList();
        }

		public IActionResult All([FromQuery]DeliverySearchQueryModel query)
        {
			(query.Deliveries, query.TotalDeliveries) = deliveryService.All(query.SearchTerm, query.Sorting, query.CurrentPage, User);

			return View(query);
        }

		public IActionResult Edit([FromQuery] string guid)
        {
			DeliveryCreateViewModel? model = null;

            try
            {
				model = deliveryService.GetDeliveryViewModelByGuid(new Guid(guid));

				deliveryService.PopulateWarehouse(model);
				deliveryService.PopulateTruck(model);
			}
			catch (FormException fe)
            {
				this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);
            }
            catch (Exception)
            {
				return RedirectToList();
            }

			var delivery = deliveryService.GetDeliveryByGuid(new Guid(guid));

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (!(User.IsInRole("Administrator") || User.IsInRole("Moderator")) 
				&& (delivery.UserId != userId || delivery.getStatus() != "Pending")
				)
			{
				return new ForbidResult();
			}

			if (model == null)
            {
				return RedirectToList();
            }

			return View(model);
        }

		[HttpPost]
		public IActionResult Edit([FromQuery] string guid, DeliveryCreateViewModel model)
        {
			deliveryService.PopulateWarehouse(model);
			deliveryService.PopulateTruck(model);

			if (!ModelState.IsValid)
			{
				return View(model);
			}
			
			var delivery = deliveryService.GetDeliveryByGuid(new Guid(guid));

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!(User.IsInRole("Administrator") || User.IsInRole("Moderator")) && (delivery.UserId != userId || delivery.getStatus() != "Pending"))
			{
				return new ForbidResult();
			}

			try
			{
				this.deliveryService.Edit(new Guid(guid), model, User);
			}
			catch (FormException e)
			{
				this.ModelState.AddModelError(e.InputName, e.ErrorMessage);
			}
			catch (Exception)
            {
				RedirectToList();
            }

			if (!ModelState.IsValid)
			{
				deliveryService.PopulateWarehouse(model);
				deliveryService.PopulateTruck(model);

				return View(model);
			}

			return RedirectToList();
		}

		[HttpPost]
		public IActionResult Delete([FromQuery] string guid)
        {
            try
            {
				deliveryService.Delete(new Guid(guid));
            }
            catch (Exception)
            {}

			return RedirectToList();
        }

		[HttpPost]
		public IActionResult Pick([FromQuery] string guid)
		{
			if (!(User.IsInRole("Administrator") || User.IsInRole("Moderator")))
			{
				return new ForbidResult();
			}

			try
			{
				deliveryService.Pick(new Guid(guid));
			}
			catch (Exception)
			{ }

			return RedirectToList();
		}

		[HttpPost]
		public IActionResult Deliver([FromQuery] string guid)
		{
			if (!(User.IsInRole("Administrator") || User.IsInRole("Moderator")))
            {
				return new ForbidResult();
			}

			try
			{
				deliveryService.Deliver(new Guid(guid));
			}
			catch (Exception)
			{ }

			return RedirectToList();
		}
	}
}
