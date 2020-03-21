using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mediumvalue_api.Model
{
    public class PricesStatistic
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AvitoValue { get; set; }
        public int YoulaValue { get; set; }
        public int YandexValue { get; set; }
        public string Name { get; set; }
    }

    public class Subcategories
    {
        public Guid Id { get; set; }
        public string AvitoValue { get; set; }
        public string YoulaValue { get; set; }
        public string Name { get; set; }
    }
}
