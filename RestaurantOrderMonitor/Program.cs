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
            Console.WriteLine("Real-time order monitoring...");
            Console.WriteLine("Statuses: New → Preparing → Ready");
            Console.WriteLine("Press any key to exit.\n");
            _timer = new Timer(CheckForOrderStatusChanges, null, 0, 5000);

            Console.ReadKey();
            _timer.Dispose();
        }

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
                {
                    SimulateStatusUpdate(context, order);
                }
                DisplayStatusChanges(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static OrderStatus? _lastStatus1 = null;
        private static OrderStatus? _lastStatus2 = null;
        private static OrderStatus? _lastStatus3 = null;

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
    }
}