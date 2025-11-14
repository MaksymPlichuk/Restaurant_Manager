using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;

class MenuManager
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        using var db = new RestaurantDbContext();
        var service = new RestaurantService(db);

        while (true)
        {
            Console.WriteLine("\n=== RESTAURANT MENU MANAGER ===");
            Console.WriteLine("1 Add a dish");
            Console.WriteLine("2 Edit a dish");
            Console.WriteLine("3 Delete a dish");
            Console.WriteLine("4 Show all dishes");
            Console.WriteLine("5 Show dishes by category");
            Console.WriteLine("6 Show popular dishes report (Statistic)");
            Console.WriteLine("7 Clear old bookings (>7 days)");
            Console.WriteLine("8 Delete booking by ID");
            Console.WriteLine("0 Exit");

            Console.Write("Your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": service.AddProduct(); break;
                case "2": service.EditProduct(); break;
                case "3": service.DeleteProduct(); break;
                case "4": service.ShowAllProducts(); break;
                case "5": service.ShowByCategory(); break;
                case "6": service.ShowStatistics(); break;
                case "7": service.ClearOldBookings(); break;
                case "8": service.DeleteBooking(); break;
                case "0":
                    Console.WriteLine("Goodbye");
                    return;
                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;
            }
        }
    }
}

public class RestaurantService
{
    private readonly RestaurantDbContext _db;

    public RestaurantService(RestaurantDbContext db)
    {
        _db = db;
    }

    public void AddProduct()
    {
        Console.WriteLine("\n--- ADD A NEW DISH ---");
        Console.Write("Dish name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Price (UAH): ");
        if (!int.TryParse(Console.ReadLine(), out int price))
        {
            Console.WriteLine("Invalid price format");
            return;
        }

        Console.Write("Category: ");
        string categoryName = Console.ReadLine() ?? "";

        var category = _db.Categories.FirstOrDefault(c => c.Name == categoryName);
        if (category == null)
        {
            category = new Category { Name = categoryName };
            _db.Categories.Add(category);
        }

        var product = new Product { Name = name, Price = price, Category = category, OrderId = 1 };
        _db.Products.Add(product);
        _db.SaveChanges();
        Console.WriteLine($"Dish '{name}' added successfully");
    }

    public void EditProduct()
    {
        ShowAllProducts();
        Console.Write("\nEnter the dish ID to edit: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var product = _db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Dish not found");
            return;
        }

        Console.WriteLine($"\nEditing: {product.Name}");
        Console.WriteLine("(Press Enter to keep the current value)\n");

        Console.Write($"Name [{product.Name}]: ");
        var newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;

        Console.Write($"Price [{product.Price}]: ");
        if (int.TryParse(Console.ReadLine(), out var newPrice)) product.Price = newPrice;

        Console.Write($"Category [{product.Category?.Name}]: ");
        var newCategoryName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newCategoryName))
        {
            var category = _db.Categories.FirstOrDefault(c => c.Name == newCategoryName);
            if (category == null)
            {
                category = new Category { Name = newCategoryName };
                _db.Categories.Add(category);
            }
            product.Category = category;
        }

        _db.SaveChanges();
        Console.WriteLine("Dish successfully updated");
    }

    public void DeleteProduct()
    {
        ShowAllProducts();
        Console.Write("\nEnter the dish ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var product = _db.Products.Find(id);
        if (product == null)
        {
            Console.WriteLine("Dish not found");
            return;
        }

        Console.Write($"Are you sure you want to delete '{product.Name}'? (yes/no): ");
        var confirm = Console.ReadLine()?.ToLower();
        if (confirm == "yes")
        {
            _db.Products.Remove(product);
            _db.SaveChanges();
            Console.WriteLine("Dish deleted");
        }
        else
        {
            Console.WriteLine("Deletion canceled");
        }
    }

    public void ShowAllProducts()
    {
        var products = _db.Products.Include(p => p.Category).ToList();
        if (products.Count == 0)
        {
            Console.WriteLine("\nThe menu is empty, add some dishes");
            return;
        }

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║               ALL MENU DISHES                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        foreach (var p in products)
        {
            Console.WriteLine($"\nID: {p.Id}");
            Console.WriteLine($"Name: {p.Name}");
            Console.WriteLine($"Price: {p.Price} ₴");
            Console.WriteLine($"Category: {p.Category?.Name ?? "No category"}");
            Console.WriteLine("───────────────────────────────────────────────");
        }
    }

    public void ShowByCategory()
    {
        Console.Write("\nEnter the category name: ");
        string categoryName = Console.ReadLine() ?? "";

        var products = _db.Products
            .Include(p => p.Category)
            .Where(p => p.Category != null && p.Category.Name == categoryName)
            .ToList();

        if (products.Count == 0)
        {
            Console.WriteLine($"\nNo dishes found in category '{categoryName}'");
            return;
        }

        Console.WriteLine($"\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine($"║     CATEGORY: {categoryName.ToUpper().PadRight(28)} ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        foreach (var p in products)
        {
            Console.WriteLine($"\n{p.Name} — {p.Price} ₴");
        }
    }

    public void ShowStatistics()
    {
        Console.WriteLine("\n--- POPULAR DISHES REPORT ---");

        var data = _db.Orders
            .Include(o => o.Products)
            .SelectMany(o => o.Products)
            .GroupBy(p => p.Name)
            .Select(g => new { Dish = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        if (data.Count == 0)
        {
            Console.WriteLine("No order data found.");
            return;
        }

        foreach (var item in data)
            Console.WriteLine($"{item.Dish} — {item.Count} orders");

        Console.Write("\nExport report? (csv/txt/skip): ");
        var format = Console.ReadLine()?.Trim().ToLower();

        if (format == "csv" || format == "txt")
            ExportReport(data, format);
    }

    private void ExportReport(IEnumerable<dynamic> data, string format)
    {
        string fileName = $"PopularDishes_{DateTime.Now:yyyyMMddHHmm}.{format}";
        char separator = format == "csv" ? ',' : '\t';

        using (var writer = new StreamWriter(fileName, false, Encoding.UTF8))
        {
            writer.WriteLine($"Dish{separator}Orders");
            foreach (var item in data)
                writer.WriteLine($"{item.Dish}{separator}{item.Count}");
        }

        Console.WriteLine($"Report saved as {fileName}");
    }

    public void ClearOldBookings()
    {
        var oldBookings = _db.Bookings
            .Where(b => b.BookingDate < DateTime.Now.AddDays(-7))
            .ToList();

        if (oldBookings.Count == 0)
        {
            Console.WriteLine("No old bookings found.");
            return;
        }

        Console.WriteLine($"Found {oldBookings.Count} old bookings. Delete them? (y/n): ");
        if ((Console.ReadLine() ?? "").Trim().ToLower() == "y")
        {
            _db.Bookings.RemoveRange(oldBookings);
            _db.SaveChanges();
            Console.WriteLine("Old bookings deleted.");
        }
        else
        {
            Console.WriteLine("Cancelled.");
        }
    }

    public void DeleteBooking()
    {
        var bookings = _db.Bookings.Include(b => b.User).ToList();
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings available.");
            return;
        }

        Console.WriteLine("\n--- ALL BOOKINGS ---");
        foreach (var b in bookings)
            Console.WriteLine($"ID : {b.Id} | User: {b.User?.Login ?? "Unknown"} | Date: {b.BookingDate}");

        Console.Write("\nEnter Booking ID to delete : ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var booking = _db.Bookings.Find(id);
        if (booking == null)
        {
            Console.WriteLine("Booking not found.");
            return;
        }

        _db.Bookings.Remove(booking);
        _db.SaveChanges();
        Console.WriteLine($"Booking #{id} deleted.");
    }
}
