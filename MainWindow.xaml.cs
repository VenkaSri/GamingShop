using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GamingShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ValidationService validationService = new ValidationService();
        TableService tableService = new TableService();
        TransactionService transactionService = new TransactionService();

        private void GamesDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGrid();
        }

        public void UpdateGrid()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var games = context.Games;
                gamesDataGrid.ItemsSource = games.ToList();
            }
        }

        private Label[] OrderSummaryLabels()
        {
            Label[] labels = { lblGameQty, lblTotalPrice, lblDiscount, lblTax, lblNetTotal };
            return labels;
        }

        private void BtnPurchase_Click(object sender, RoutedEventArgs e)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            GetGamesGridInfo(purchaseOrder);
            GetCustomerName(purchaseOrder);
            GetQuantity(purchaseOrder);
            validationService.ValidatePurchase(purchaseOrder, OrderSummaryLabels());
            Trace.WriteLine(validationService.IsOrderPlaced);
            if (validationService.IsOrderPlaced) UpdateGrid();
        }

        private void GetGamesGridInfo(PurchaseOrder purchaseOrder)
        {
            if (gamesDataGrid.SelectedItems.Count > 0)
                purchaseOrder.GameSelected = true;
            else
                purchaseOrder.GameSelected = false;
        }

        private void GetCustomerName(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.CustomerName = txtCustomerName.Text;
        }

        private void GetQuantity(PurchaseOrder purchaseOrder)
        {
            purchaseOrder.Quantity = txtQuantity.Text;
        }

        private void gamesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { 
            Game game = (Game)gamesDataGrid.SelectedItem;
            if (game != null) 
                validationService.GameId = game.GameId;
            else 
                return;
            lblGamePrice.Content = game.Price.ToString("C");
            ClearOtherLabels();
        }

        private void btnLoadNewQtys_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 5; i++)
            {
                Game game = new Game();
                game.GameId = i;
                game.Stock = RandomNumber();
                tableService.UpdateGamesInventory(game);
            }
            UpdateGrid();
        }

        private void ClearOtherLabels()
        {
            foreach (var item in OrderSummaryLabels())
            {
                item.Content = "";
            }
        }

        private int RandomNumber()
        {
            Random random = new Random();
            int num = random.Next(0, 21);
            return num;
        }

        private void BtnClearFields_Click(object sender, RoutedEventArgs e)
        {
            lblGamePrice.Content = "";
            ClearOtherLabels();
            txtCustomerName.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            UpdateGrid();
        }

        private void lstCustomers_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCustomersList();
        }

        public void UpdateCustomersList()
        {
            using (var context = new GameShoppingDBEntities())
            {
                var customers = from c in context.Customers
                                select c.CustomerName;
                lstCustomers.ItemsSource = customers.ToList();
            }
        }

        private void lstCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name;
            try
            {
                name = lstCustomers.SelectedItem.ToString();
                int customerId = tableService.GetCustomerId(name);
                transactionService.ShowTransactions(customerId, transactionsDataGrid);
            }
            catch (Exception err)
            {
                Trace.WriteLine(err);
                return;
            }  
        }

        private void btnDeleteCustomers_Click(object sender, RoutedEventArgs e)
        {
            tableService.DeleteAllOrders();
            tableService.DeleteAllCustomers();
            transactionsDataGrid.ItemsSource = null;
            allTransactionsDataGrid.ItemsSource = null;
            UpdateCustomersList();
        }
        private void allTransactionsDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            transactionService.ShowAllTransactions(allTransactionsDataGrid);
        }
    }
}
