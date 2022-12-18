using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GamingShop
{
    public class TransactionService
    {
        public void ShowTransactions(int customerId, DataGrid dataGrid)
        {
            using (var context = new GameShoppingDBEntities())
            {
                var transactions = (from o in context.Orders
                                    where o.CustomerId == customerId
                                    select new
                                    {

                                        o.OrderId,
                                        o.Date,
                                        o.Customer.CustomerName,
                                        o.Game.GameName,
                                        o.Game.Price,
                                        o.Quantity,
                                        o.Discount,
                                    }).AsEnumerable()
                                   .Select(t => new
                                   {
                                       t.OrderId,
                                       t.Date,
                                       t.CustomerName,
                                       t.GameName,
                                       Price = t.Price.ToString("C"),
                                       t.Quantity,
                                       Discount = t.Discount.ToString("C"),
                                       Tax = (((t.Price * t.Quantity) - t.Discount) * (13.0 / 100)).ToString("C"),
                                       Total = (((t.Price * t.Quantity) - t.Discount) +
                                                  (((t.Price * t.Quantity) - t.Discount) * (13.0 / 100))).ToString("C")
                                   }).ToList();
                dataGrid.ItemsSource = transactions.ToList();
                
            }
        }

        public void ShowAllTransactions(DataGrid dataGrid)
        {
            using (var context = new GameShoppingDBEntities())
            {
                var transactions = (from o in context.Orders
                                    select new
                                    {
                                        o.OrderId,
                                        o.Date,
                                        o.Customer.CustomerName,
                                        o.Game.GameName,
                                        o.Game.Price,
                                        o.Quantity,
                                        o.Discount,
                                    }).AsEnumerable()
                                   .Select(t => new
                                   {
                                       t.OrderId,
                                       t.Date,
                                       t.CustomerName,
                                       t.GameName,
                                       Price = t.Price.ToString("C"),
                                       t.Quantity,
                                       Discount = t.Discount.ToString("C"),
                                       Tax = (((t.Price * t.Quantity) - t.Discount) * (13.0 / 100)).ToString("C"),
                                       Total = (((t.Price * t.Quantity) - t.Discount) +
                                                  (((t.Price * t.Quantity) - t.Discount) * (13.0 / 100))).ToString("C")
                                   }).ToList();
                dataGrid.ItemsSource = transactions.ToList();
            }
        }
    }
}
