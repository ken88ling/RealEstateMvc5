using RealEstateMvc5.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace RealEstateMvc5.Rentals
{
    public class RentalsController : Controller
    {
        public readonly RealEstateContext Context = new RealEstateContext();
        // GET: /<controller>/
        public ActionResult Post()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Post(PostRental postRental)
        {
            var rental = new Rental(postRental);
            Context.Rentals.Insert(rental);
            return RedirectToAction("Index");
        }
        
        public ActionResult Index(RentalsFilter filters)
        {
            var rentals = FilterRentals(filters);

            var model = new RentalsList()
            {
                Rentals = rentals,
                Filters = filters
            };

            return View(model);
        }

        private MongoCursor<Rental> FilterRentals(RentalsFilter filters)
        {
            if (!filters.PriceLimit.HasValue)
            {
                return Context.Rentals.FindAll();
            }
            var query = Query<Rental>.LTE(r => r.Price, filters.PriceLimit);
            return Context.Rentals.Find(query);
        }

        public ActionResult AdjustPrice(string id)
        {
            var rental = GetRental(id);
            return View(rental);
        }

        private Rental GetRental(string id)
        {
            var rental = Context.Rentals.FindOneById(new ObjectId(id));
            return rental;
        }

        [HttpPost]
        public ActionResult AdjustPrice(string id, AdjustPrice adjustPrice)
        {
            var rental = GetRental(id);
            rental.AdjustPrice(adjustPrice);
            Context.Rentals.Save(rental);
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            Context.Rentals.Remove(Query.EQ("_id", new ObjectId(id)));
            return RedirectToAction("Index");
        }
    }
}
