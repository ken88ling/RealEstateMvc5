using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateMvc5.Rentals
{
    public class RentalsFilter
    {
        public decimal? PriceLimit { get; set; }
        public int? MinimumRooms { get; set; }

    }
}

