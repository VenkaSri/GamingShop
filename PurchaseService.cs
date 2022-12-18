using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GamingShop
{
    public class PurchaseService
    {

        private TableService tableService = new TableService();
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Discount { get; set; }
        private double Tax { get; set; }
        public double Total { get; set; }
        public double NetTotal { get; set; }

        public void PlaceOrder(PurchaseOrder purchaseOrder, Label[] labels)
        {
            CalculateTotalPrice(purchaseOrder);
            CheckForDiscount(purchaseOrder);
            CalculateNetTotal(purchaseOrder);
            DisplayPrices(labels);
            tableService.UpdateGamesInventory(int.Parse(purchaseOrder.Quantity), purchaseOrder.GameId);
            CreateOrder(purchaseOrder);
            OrderConfirmation();
        }

        public void SetGamePrice(PurchaseOrder purchaseOrder)
        {
            using (var context = new GameShoppingDBEntities())
            {
                var game = context.Games.Find(purchaseOrder.GameId);
                Price = game.Price;
            }
        }

        public void CalculateTotalPrice(PurchaseOrder purchaseOrder)
        {
            SetGamePrice(purchaseOrder);
            Quantity = int.Parse(purchaseOrder.Quantity);
            Total = Price * Quantity; 
        }

        public void CreateOrder(PurchaseOrder purchaseOrder)
        {
            using (var context = new GameShoppingDBEntities())
            {
                Order order = new Order();
                order.CustomerId = purchaseOrder.CustomerId;
                order.Date = DateTime.Now.Date;
                order.GameId = purchaseOrder.GameId;
                order.Discount = Discount;
                order.Quantity = Quantity;

                context.Orders.Add(order);
                context.SaveChanges();
            }
        }
        public void CheckForDiscount(PurchaseOrder purchaseOrder)
        {
            int qty = int.Parse(purchaseOrder.Quantity);
            Trace.WriteLine("Qty = " + qty);
            if (qty >= 5)
            {
                Discount = Total * (10.0 / 100);
            }         
        }

        public void CalculateNetTotal(PurchaseOrder purchaseOrder) 
        {
            Tax = (Total - Discount) * (13.0 / 100);
            NetTotal = (Total - Discount) + Tax;
        }

        public void DisplayPrices(Label[] labels)
        {
            labels[0].Content = Quantity;
            labels[1].Content = Total.ToString("C");
            labels[2].Content = Discount.ToString("C");
            labels[3].Content = Tax.ToString("C");
            labels[4].Content = NetTotal.ToString("C");
        }

        public void OrderConfirmation()
        {

            MessageBox.Show("Your order has been placed!", "Thank you!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
