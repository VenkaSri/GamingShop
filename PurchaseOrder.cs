using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamingShop
{
    public class PurchaseOrder
    {
        public string CustomerName { get; set; }
        public string Quantity { get; set; }
        public bool GameSelected { get; set; }
        public int GameId { get; set; }
        public int CustomerId { get; set; }
        public bool OrderValidated { get; set; }
    }
}
