using System;
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

        while (true)
        {

            Console.WriteLine("\n=== RESTAURANT MENU MANAGER ===");

            Console.WriteLine("1 Add a dish");
            Console.WriteLine("2 Edit a dish");
            Console.WriteLine("3 Delete a dish");
            Console.WriteLine("4 Show all dishes");
            Console.WriteLine("5 Show dishes by category");
            Console.WriteLine("0 Exit");

            Console.Write("Your choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {

                case "1": AddProduct(db); break;
                case "2": EditProduct(db); break;
                case "3": DeleteProduct(db); break;
                case "4": ShowAllProducts(db); break;
                case "5": ShowByCategory(db); break;

                case "0":
                    Console.WriteLine("Goodbye");
                    return;

                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;     
            }
        }
    }

    static void AddProduct(RestaurantDbContext db)
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

        var category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
        if (category == null)
        {
            category = new Category { Name = categoryName };
            db.Categories.Add(category);
        }

        var product = new Product
        {
            Name = name,
            Price = price,
            Category = category
        };

        db.Products.Add(product);
        db.SaveChanges();

        Console.WriteLine($"Dish '{name}' added successfully");
    }

    static void EditProduct(RestaurantDbContext db)
    {
        ShowAllProducts(db);
        Console.Write("\nEnter the dish ID to edit: ");

        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            Console.WriteLine("Dish not found");
            return;
        }

        Console.WriteLine($"\nEditing: {product.Name}");
        Console.WriteLine("(Press Enter to keep the current value)\n");
        
        Console.Write($"Name [{product.Name}]: ");
        
        var newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName))
            product.Name = newName;

        Console.Write($"Price [{product.Price}]: ");

        if (int.TryParse(Console.ReadLine(), out var newPrice))
            product.Price = newPrice;

        Console.Write($"Category [{product.Category?.Name}]: ");

        var newCategoryName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newCategoryName))
        {
            var category = db.Categories.FirstOrDefault(c => c.Name == newCategoryName);
            if (category == null)
            {
                category = new Category { Name = newCategoryName };
                db.Categories.Add(category);
            }
            product.Category = category;
        }
        db.SaveChanges();

        Console.WriteLine("Dish successfully updated");
    }

    static void DeleteProduct(RestaurantDbContext db)
    {
        ShowAllProducts(db);
        
        Console.Write("\nEnter the dish ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        var product = db.Products.Find(id);

        if (product == null)
        {
            Console.WriteLine("Dish not found");
            return;
        }

        Console.Write($"Are you sure you want to delete '{product.Name}'? (yes/no): ");
        var confirm = Console.ReadLine()?.ToLower();

        if (confirm == "yes" || confirm == "no")
        {
            db.Products.Remove(product);
            db.SaveChanges();
            Console.WriteLine("Dish deleted");
        }

        else
        {
            Console.WriteLine("Deletion canceled");
        }
    }

    static void ShowAllProducts(RestaurantDbContext db)
    {
        var products = db.Products.Include(p => p.Category).ToList();

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

    static void ShowByCategory(RestaurantDbContext db)
    {
        Console.Write("\nEnter the category name: ");
        string categoryName = Console.ReadLine() ?? "";

        var products = db.Products
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

}

