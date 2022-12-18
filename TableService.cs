using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace GamingShop
{
    public class TableService 
    {
        public Customer CheckIfCustomerExists(string name)
        {   
            using (var context = new GameShoppingDBEntities())
            {
                var customer = (from c in context.Customers
                                where c.CustomerName == name
                                select c).FirstOrDefault();
                return customer;
            }  
        }

        public void UpdateCustomerTable(string name)
        {
            using (var context = new GameShoppingDBEntities())
            { 
                Customer customer = new Customer();
                customer.CustomerName = name;
                context.Customers.Add(customer);
                context.SaveChanges();   
            }
        }

        public void UpdateGamesInventory(int qty, int gameId)
        {
            using (var context = new GameShoppingDBEntities())
            {
                Game game = new Game();
                game = context.Games.Find(gameId);
                game.Stock -= qty;
                context.SaveChanges();
            }    
        }

        public void UpdateGamesInventory(Game game)
        {
            using (var context = new GameShoppingDBEntities())
            {
                Game newGame = new Game();
                newGame = context.Games.Find(game.GameId);
                newGame.Stock = game.Stock;

                context.SaveChanges();
            }
        }

        public void DeleteAllOrders()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var orders = from o in context.Orders
                             select o;
                context.Orders.RemoveRange(orders);
                context.SaveChanges();
            }
        }

        public void DeleteAllCustomers()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var customers = from c in context.Customers
                             select c;
                context.Customers.RemoveRange(customers);
                context.SaveChanges();
            }
        }

        public int GetCustomerId(string name)
        {  
            using (var context = new GameShoppingDBEntities())
            {
                var customer = (from c in context.Customers
                                where c.CustomerName == name
                                select c).FirstOrDefault();
                return customer.CustomerId;
            }       
        }
    }
}
