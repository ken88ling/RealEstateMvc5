using RealEstateMvc5.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

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

            //var rentals = FilterRentals(filters)
            //    .SetSortOrder(SortBy<Rental>.Ascending(r => r.Price)); //for MongoCursor<Rental>

            var model = new RentalsList()
            {
                Rentals = rentals,
                Filters = filters
            };

            return View(model);
        }

        private IEnumerable<Rental> FilterRentals(RentalsFilter filters)
        {
            IQueryable<Rental> rentals = Context.Rentals.AsQueryable()
                .OrderBy(r => r.Price);

            if (filters.MinimumRooms.HasValue)
            {
                rentals = rentals
                    .Where(r => r.NumberOfRooms >= filters.MinimumRooms.Value);
                //var minRoom = Query<Rental>.LTE(r=>r.NumberOfRooms,filters.MinimumRooms);
                //rentals = rentals
                //    .Where(r => minRoom.Inject());
            }

            if (filters.PriceLimit.HasValue)
            {
                var query = Query<Rental>.LTE(r => r.Price, filters.PriceLimit);
                rentals = rentals
                    .Where(r => query.Inject());
            }

            return rentals;
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

        public string PriceDistribution()
        {
            return new QueryPriceDistribution()
                .Run(Context.Rentals)
                .ToJson();
        }

        public ActionResult AttachImage(string id)
        {
            var rental = GetRental(id);
            return View(rental);
        }

        [HttpPost]
        public ActionResult AttachImage(string id, HttpPostedFileBase file)
        {
            var rental = GetRental(id);
            if (rental.HasImage())
            {
                DeleteImage(rental);
            }
            StoreImage(file, rental);
            return RedirectToAction("Index");
        }

        private void DeleteImage(Rental rental)
        {
            Context.Database.GridFS.DeleteById(new ObjectId(rental.ImageId));
            rental.ImageId = null;
            Context.Rentals.Save(rental);
        }

        private void StoreImage(HttpPostedFileBase file, Rental rental)
        {
            var imageId = ObjectId.GenerateNewId();
            rental.ImageId = imageId.ToString();
            Context.Rentals.Save(rental);
            var options = new MongoGridFSCreateOptions()
            {
                Id = imageId,
                ContentType = file.ContentType
            };

            Context.Database.GridFS.Upload(file.InputStream, file.FileName, options);
        }

        public ActionResult GetImage(string id)
        {
            var image = Context.Database.GridFS
                .FindOneById(new ObjectId(id));

            if (image == null)
            {
                return HttpNotFound();
            }

            return File(image.OpenRead(), image.ContentType);
        }
    }
}
