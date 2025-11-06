using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;

namespace RestaurantOrderMonitor.ConsoleApp
{
    class Program
    {
        private static Timer _timer; 

        static void Main(string[] args)
        {
            Console.WriteLine("=== Restaurant Order Monitor ===");
            Console.WriteLine("1. Auto monitoring (every 5 sec)");
            Console.WriteLine("2. Manual status change");
            Console.Write("Choose mode: ");
            var mode = Console.ReadLine();

            if (mode == "2")
                ManualMode();
            else
                AutoMode();
        }

        #region Auto Mode
        private static void AutoMode()
        {
            Console.WriteLine("Real-time order monitoring...");
            Console.WriteLine("Statuses: New → Preparing → Ready");
            Console.WriteLine("Press any key to exit.\n");

            _timer = new Timer(CheckForOrderStatusChanges, null, 0, 5000);

            Console.ReadKey();
            _timer?.Dispose(); 
        }

        private static OrderStatus? _lastStatus1 = null;
        private static OrderStatus? _lastStatus2 = null;
        private static OrderStatus? _lastStatus3 = null;

        private static void CheckForOrderStatusChanges(object state)
        {
            try
            {
                using var context = new RestaurantDbContext();

                var orders = context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Products)
                    .ToList();

                foreach (var order in orders)
                    SimulateStatusUpdate(context, order);

                DisplayStatusChanges(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void DisplayStatusChanges(RestaurantDbContext context)
        {
            var order1 = context.Orders.Find(1);
            var order2 = context.Orders.Find(2);
            var order3 = context.Orders.Find(3);
            if (order1 != null && order1.Status != _lastStatus1)
            {
                Console.WriteLine($"[CHANGE] Order #{order1.Id} ({order1.User?.Login}) — status: {order1.Status}");
                _lastStatus1 = order1.Status;
            }
            if (order2 != null && order2.Status != _lastStatus2)
            {
                Console.WriteLine($"[CHANGE] Order #{order2.Id} ({order2.User?.Login}) — status: {order2.Status}");
                _lastStatus2 = order2.Status;
            }
            if (order3 != null && order3.Status != _lastStatus3)
            {
                Console.WriteLine($"[CHANGE] Order #{order3.Id} ({order3.User?.Login}) — status: {order3.Status}");
                _lastStatus3 = order3.Status;
            }
        }

        private static readonly Random _random = new Random();

        private static void SimulateStatusUpdate(RestaurantDbContext context, Order order)
        {
            if (order.Status == OrderStatus.New && _random.Next(100) < 30)
            {
                order.Status = OrderStatus.Preparing;
                context.SaveChanges();
            }
            else if (order.Status == OrderStatus.Preparing && _random.Next(100) < 40)
            {
                order.Status = OrderStatus.Ready;
                context.SaveChanges();
            }
        }
        #endregion

        #region Manual Mode
        private static void ManualMode()
        {
            while (true)
            {
                Console.Clear();
                ShowAllOrders(); 

                Console.WriteLine("\n=== Manual Status Change ===");
                Console.Write("Enter Order ID (or 0 to exit): ");
                if (!int.TryParse(Console.ReadLine(), out int orderId) || orderId == 0)
                    break;

                using var context = new RestaurantDbContext();
                var order = context.Orders
                    .Include(o => o.User)
                    .Include(o => o.Products)
                    .FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                {
                    Console.WriteLine("Order not found!");
                    Console.ReadKey();
                    continue;
                }
                Console.WriteLine($"\nOrder #{order.Id}");
                Console.WriteLine($"User: {order.User?.Login}");
                Console.WriteLine($"Current status: {order.Status}");
                Console.WriteLine($"Products: {string.Join(", ", order.Products.Select(p => p.Name))}");

                Console.WriteLine("\nNew status:");
                Console.WriteLine("1. New");
                Console.WriteLine("2. Preparing");
                Console.WriteLine("3. Ready");
                Console.Write("Choose (1-3): ");
                var choice = Console.ReadLine();

                OrderStatus newStatus = order.Status;
                switch (choice)
                {
                    case "1": newStatus = OrderStatus.New; break;
                    case "2": newStatus = OrderStatus.Preparing; break;
                    case "3": newStatus = OrderStatus.Ready; break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        Console.ReadKey();
                        continue;
                }
                if (newStatus == order.Status)
                    Console.WriteLine("Status unchanged.");
                else
                {
                    order.Status = newStatus;
                    context.SaveChanges();
                    Console.WriteLine($"Status updated to: {newStatus}");
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ShowAllOrders()
        {
            using var context = new RestaurantDbContext();
            var orders = context.Orders
                .Include(o => o.User)
                .Include(o => o.Products)
                .OrderBy(o => o.Id)
                .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("No orders found.");
                return;
            }
            Console.WriteLine("ID | User | Status | Products");
            Console.WriteLine(new string('-', 50));
            foreach (var o in orders)
            {
                var products = string.Join(", ", o.Products.Select(p => p.Name));
                Console.WriteLine($"{o.Id,2} | {o.User?.Login,-10} | {o.Status,-10} | {products}");
            }
        }
        #endregion
    }
}