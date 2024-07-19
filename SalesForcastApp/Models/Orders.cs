using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SalesForecastApp.Models
{
    public class Orders
    {
        public string orderid {  get; set; }
        public string customerid { get; set; }
        public string customername {  get; set; }
        public DateTime orderdate { get; set; }
        public DateTime shipdate { get; set; }
        public string shipmode { get; set; }
        public string segment {  get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string postalcode { get; set; }
    }

}
