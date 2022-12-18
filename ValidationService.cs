using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Controls;

namespace GamingShop
{
    public class ValidationService
    {
        private PurchaseOrder _purchaseOrder;
        private TableService tableService = new TableService();
        private PurchaseService purchaseService = new PurchaseService();
        public int GameId { get; set; }
        public double DiscountPercent { get; set; }
        public bool IsOrderPlaced { get; set; }

        public ValidationService() { }
        public ValidationService(PurchaseOrder purchaseOrder)
        {
            _purchaseOrder = purchaseOrder;
        }

        public void ValidatePurchase(PurchaseOrder purchaseOrder, Label[] labels)
        {
            _purchaseOrder = purchaseOrder;
            if (!_purchaseOrder.OrderValidated) IsGameSelected(_purchaseOrder.GameSelected);
            if (!_purchaseOrder.OrderValidated) CheckCustomerName(_purchaseOrder.CustomerName);
            if (!_purchaseOrder.OrderValidated) CheckQuantity(_purchaseOrder.Quantity);
            if (!_purchaseOrder.OrderValidated)
            {
                purchaseService.PlaceOrder(_purchaseOrder, labels);
                IsOrderPlaced = true;
            }  
        }

        private void IsGameSelected(bool boolvalue)
        {
            if (boolvalue)
            {
                _purchaseOrder.GameId = GameId;
                return;
            }
            else
            {
                _purchaseOrder.OrderValidated = true;
                MessageBox.Show("You haven't selected a game!", "Game Not Selected",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }         
        }  

        private void CheckCustomerName(string name)
        {
            if (name.Equals(""))
            {
                _purchaseOrder.OrderValidated = true;
                MessageBox.Show("You didn't enter any name!", "Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else 
            {
                CheckAndUpdateCustomersTable(name);
            }
        }

        private void CheckAndUpdateCustomersTable(string name)
        {
            Customer customer = tableService.CheckIfCustomerExists(name);
            if (customer == null) 
            {
                tableService.UpdateCustomerTable(name);
            }
            _purchaseOrder.CustomerId = tableService.GetCustomerId(name);
        }

        private void CheckQuantity(string qty)
        {
            int quantity;
            if (qty.Equals(""))
            {
                _purchaseOrder.OrderValidated = true;
                MessageBox.Show("You didn't enter any quantities!", "Quantity", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!int.TryParse(qty, out quantity))
            {
                _purchaseOrder.OrderValidated = true;
                MessageBox.Show("Invalid input for quantites!", "Quantity", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            quantity = int.Parse(qty);
            CheckOrders(quantity);
        }

        public void CheckOrders(int qty)
        {
            using (var context = new GameShoppingDBEntities())
            {
                var game = context.Games.Find(GameId);
                if (game.Stock == 0)
                {
                    _purchaseOrder.OrderValidated = true;
                    MessageBox.Show($"This game is out of stock at the moment, please check back later", "Out of Stock", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (qty <= 0)
                {
                    _purchaseOrder.OrderValidated = true;
                    MessageBox.Show($"You have entered an quantity thats less than or equal 0. " +
                        $"\nYou have atleast purchase one!", "Quantity Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (qty > game.Stock)
                {
                    _purchaseOrder.OrderValidated = true;
                    MessageBox.Show($"Not enough quantity available, try again! " +
                        $"\nNumber of {game.GameName} units in stock: {game.Stock}", "Not enought in stock", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
            }
        }
    }
}
