using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateMvc5.Rentals
{
    public class RentalsList
    {
        public IEnumerable<Rental> Rentals { get; set; }
        public RentalsFilter Filters { get; set; }
    }
}
