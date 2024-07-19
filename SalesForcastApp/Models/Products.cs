using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SalesForecastApp.Models
{
    public class Products
    {
      
        public string productid {  get; set; }
        public string productname { get; set; }
        public string orderid {  get; set; }
        public string category {  get; set; }
        public string subcategory { get; set; }
        public decimal sales { get; set; }
        public int quantity {  get; set; }
        public Decimal discount { get; set; }
        public decimal profit { get; set; }
    }
}
