namespace CargoExpress.Controllers
{
    using CargoExpress.Core.Contracts;
    using CargoExpress.Core.Exceptions;
    using CargoExpress.Core.Models;
    using Microsoft.AspNetCore.Mvc;

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

			return View(model);
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
            catch (FormException e)
            {
				this.ModelState.AddModelError(e.InputName, e.ErrorMessage);
            }

			if (!ModelState.IsValid)
			{
				deliveryService.PopulateWarehouse(model);
				return View(model);
			}

			return RedirectToList();
        }

		public IActionResult All([FromQuery]DeliverySearchQueryModel query)
        {
			(query.Deliveries, query.TotalDeliveries) = deliveryService.All(query.SearchTerm, query.Sorting, query.CurrentPage);

			return View(query);
        }

		public IActionResult Edit([FromQuery] string guid)
        {
			DeliveryCreateViewModel? model = null;

            try
            {
				model = deliveryService.GetDeliveryViewModelByGuid(new Guid(guid));
				deliveryService.PopulateWarehouse(model);
            }
			catch (FormException fe)
            {
				this.ModelState.AddModelError(fe.InputName, fe.ErrorMessage);
            }
            catch (Exception)
            {
				return RedirectToList();
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

			if (!ModelState.IsValid)
			{
				return View(model);
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
    }
}
