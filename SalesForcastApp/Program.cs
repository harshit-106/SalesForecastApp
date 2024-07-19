// Program.cs
using SalesForecastApp.Data;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using System.Data;

public class Program
{
    public static void Main(string[] args)
    {
        
        using var context = new SalesForecastContext();

        // Query Sales for a Specific Year
        Console.WriteLine("Enter the year for sales query:");
        int year = int.Parse(Console.ReadLine());
        QuerySalesForYear(context, year);

        // Apply Percentage Increase
        Console.WriteLine("Enter the percentage increase:");
        decimal percentage = decimal.Parse(Console.ReadLine());
        var forecastData = ApplyPercentageIncrease(context, year, percentage);

        // Export Forecast to CSV
        ExportForecastToCsv(forecastData, "forecast.csv");
        Console.WriteLine("Forecast data has been exported to 'forecast.csv'.");
    }


    private static NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(@"Server=localhost;Port=5432;Database=SalesForcasting;User Id=postgres;Password=admin;");
    }
   

    public static void QuerySalesForYear(SalesForecastContext context, int year)
    {
        // Step 1: Filter orders by year and exclude those in OrdersReturns
        var filteredOrders = context.Orders
            .Where(o => o.orderdate.Year == year)
            .GroupJoin(
                context.Returns,
                o => o.orderid,
                r => r.orderid,
                (o, r) => new { order = o, returns = r })
            .SelectMany(
                or => or.returns.DefaultIfEmpty(),
                (or, r) => new { or.order, Return = r })
            .Where(or => or.Return == null)
            .Select(or => new { or.order.orderid, or.order.state });

        // Step 2: Join the filtered orders with the Products table
        var salesData = filteredOrders
            .Join(
                context.Products,
                fo => fo.orderid,
                p => p.orderid,
                (fo, p) => new { fo.state, p.sales })
            .GroupBy(x => x.state)
            .Select(g => new
            {
                state = g.Key,
                totalsales = g.Sum(x => x.sales)
            })
            .ToList();

        // Display total sales and breakdown by state
        var totalSales = salesData.Sum(s => s.totalsales);
        Console.WriteLine($"Total Sales for {year}: {totalSales}");
        foreach (var data in salesData)
        {
            Console.WriteLine($"State: {data.state}, Sales: {data.totalsales}");
        }
    }

    // ApplyPercentageIncrease Method
    public static List<SalesData> ApplyPercentageIncrease(SalesForecastContext context, int year, decimal percentage)
    {
        var salesData = context.Orders
            .Where(o => o.orderdate.Year == year)
            .Join(context.Products,
                o => o.orderid,
                p => p.orderid,
                (o, p) => new { o.state, p.sales })
            .GroupBy(x => x.state)
            .Select(g => new
            {
                State = g.Key,
                TotalSales = g.Sum(x => x.sales) - context.Returns
                    .Where(r => r.orderid == g.Key)
                    .Join(context.Products,
                        r => r.orderid,
                        p => p.orderid,
                        (r, p) => p.sales)
                    .Sum()
            })
            .ToList();

        var forecastData = salesData
            .Select(data => new SalesData
            {
                State = data.State,
                PercentageIncrease = percentage,
                SalesValue = data.TotalSales * (1 + percentage / 100)
            })
            .ToList();

        // Display incremented sales
        foreach (var data in forecastData)
        {
            Console.WriteLine($"State: {data.State}, Incremented Sales: {data.SalesValue}");
        }

        return forecastData; // Corrected: Return the list of SalesData
    }

    // ExportForecastToCsv Method
    public static void ExportForecastToCsv(List<SalesData> salesData, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("State,Percentage Increase,Sales Value");
            foreach (var data in salesData)
            {
                writer.WriteLine($"{data.State},{data.PercentageIncrease},{data.SalesValue}");
            }
        }
    }
}

public class SalesData
{
    public string State { get; set; }
    public decimal PercentageIncrease { get; set; }
    public decimal SalesValue { get; set; }
}


