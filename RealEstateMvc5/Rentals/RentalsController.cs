using RealEstateMvc5.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MongoDB.Bson;

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
        
        public ActionResult Index()
        {
            var rentals = Context.Rentals.FindAll();
            return View(rentals);
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
        
    }
}
