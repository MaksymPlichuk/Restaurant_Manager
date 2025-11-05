using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
    public List<Booking> Bookings { get; set; } = new List<Booking>();
    public List<Order> Orders { get; set; } = new List<Order>();
    public List<Review> Reviews { get; set; } = new List<Review>();
}

public class Booking
{
    public int Id { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public DateTime BookingDate { get; set; }
    public int NumberOfTables { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Product> Products { get; set; } = new List<Product>();
}

public class Order
{
    public int Id { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<Product> Products { get; set; } = new List<Product>();
    public bool IsActive { get; set; } = true;
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Category Category { get; set; }
    public int CategoryId { get; set; }
    public int Price { get; set; }
    public bool IsSold { get; set; } = false;
}

public class Review
{
    public int Id { get; set; }
    public User User { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = "";
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
}

public class ProductService
{
    private List<Product> products = new List<Product>();
    private List<Order> orders = new List<Order>();
    private List<Category> categories = new List<Category>();
    private int nextProductId = 1;
    private int nextCategoryId = 1;
    private int nextOrderId = 1;

    public void AddProduct()
    {
        Console.Write("Enter dish name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Enter price: ");
        if (!int.TryParse(Console.ReadLine(), out int price))
        {
            Console.WriteLine("Invalid price");
            return;
        }
        Console.Write("Enter category (or leave empty): ");
        string categoryName = Console.ReadLine() ?? "";
        Category category = null;
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            category = categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (category == null)
            {
                category = new Category { Id = nextCategoryId++, Name = categoryName };
                categories.Add(category);
            }
        }
        var product = new Product { Id = nextProductId++, Name = name, Price = price, Category = category };
        products.Add(product);
        if (category != null) category.Products.Add(product);
        Console.WriteLine("Dish added successfully");
    }

    public void EditProduct()
    {
        ShowAllProducts();
        Console.Write("Enter dish ID to edit: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Dish not found");
            return;
        }
        Console.WriteLine($"Editing {product.Name} (press Enter to keep)");
        Console.Write($"Name [{product.Name}]: ");
        var newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName)) product.Name = newName;
        Console.Write($"Price [{product.Price}]: ");
        var priceInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(priceInput) && int.TryParse(priceInput, out int newPrice)) product.Price = newPrice;
        Console.Write($"Category [{product.Category?.Name ?? "none"}]: ");
        var newCat = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newCat))
        {
            var category = categories.FirstOrDefault(c => c.Name.Equals(newCat, StringComparison.OrdinalIgnoreCase));
            if (category == null)
            {
                category = new Category { Id = nextCategoryId++, Name = newCat };
                categories.Add(category);
            }
            product.Category?.Products.Remove(product);
            product.Category = category;
            category.Products.Add(product);
        }
        Console.WriteLine("Dish updated");
    }

    public void DeleteProduct()
    {
        ShowAllProducts();
        Console.Write("Enter dish ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }
        var product = products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            Console.WriteLine("Dish not found");
            return;
        }
        bool hasActiveOrder = orders.Any(o => o.IsActive && o.Products.Any(p => p.Id == id));
        if (hasActiveOrder)
        {
            Console.WriteLine("Cannot delete this dish it is part of an active order");
            return;
        }
        Console.Write($"Are you sure you want to delete '{product.Name}' yes/no: ");
        var confirm = (Console.ReadLine() ?? "").Trim().ToLower();
        if (confirm == "yes" || confirm == "y")
        {
            products.Remove(product);
            product.Category?.Products.Remove(product);
            Console.WriteLine("Dish deleted");
        }
        else
        {
            Console.WriteLine("Deletion canceled");
        }
    }

    public void ShowAllProducts()
    {
        if (!products.Any())
        {
            Console.WriteLine("No dishes available");
            return;
        }
        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id} {p.Name} {p.Price} UAH Category:{p.Category?.Name ?? "none"} Sold:{p.IsSold}");
        }
    }

    public void CreateOrder()
    {
        if (!products.Any())
        {
            Console.WriteLine("No dishes to order");
            return;
        }
        var order = new Order { Id = nextOrderId++, OrderDate = DateTime.Now, IsActive = true };
        while (true)
        {
            ShowAllProducts();
            Console.Write("Enter dish ID to add to order or 0 to finish: ");
            if (!int.TryParse(Console.ReadLine(), out int id) || id == 0) break;
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                Console.WriteLine("Dish not found");
                continue;
            }
            order.Products.Add(product);
            Console.WriteLine($"Added {product.Name}");
        }
        if (order.Products.Any())
        {
            orders.Add(order);
            Console.WriteLine($"Order {order.Id} created with {order.Products.Count} items");
        }

        else
        {
            Console.WriteLine("Empty order not created");
        }
    }

    public void CloseOrder()
    {
        if (!orders.Any())
        {
            Console.WriteLine("No orders");
            return;
        }
        Console.Write("Enter order ID to close: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID");
            return;
        }
        var order = orders.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            Console.WriteLine("Order not found");
            return;
        }
        order.IsActive = false;
        foreach (var p in order.Products) p.IsSold = true;
        Console.WriteLine($"Order {id} closed");
    }

    public void ShowSalesStatistics()
    {
        var sold = orders.Where(o => !o.IsActive).SelectMany(o => o.Products).ToList();
        if (!sold.Any())
        {
            Console.WriteLine("No sales data yet");
            return;
        }
        int count = sold.Count;
        int total = sold.Sum(p => p.Price);
        double avg = sold.Average(p => p.Price);
        Console.WriteLine("Sales statistics");
        Console.WriteLine($"Sold dishes {count}");
        Console.WriteLine($"Total revenue {total} UAH");
        Console.WriteLine($"Average price {avg:F2} UAH");
    }
    //
    public void SeedExampleData()
    {
        if (products.Any()) return;
        var c1 = new Category { Id = nextCategoryId++, Name = "Main" };
        var c2 = new Category { Id = nextCategoryId++, Name = "Dessert" };
        categories.Add(c1);
        categories.Add(c2);
        var p1 = new Product { Id = nextProductId++, Name = "Borscht", Price = 120, Category = c1 };
        var p2 = new Product { Id = nextProductId++, Name = "Varenyky", Price = 90, Category = c1 };
        var p3 = new Product { Id = nextProductId++, Name = "Syrnyk", Price = 70, Category = c2 };
        products.AddRange(new[] { p1, p2, p3 });
        c1.Products.AddRange(new[] { p1, p2 });
        c2.Products.Add(p3);
        var order = new Order { Id = nextOrderId++, OrderDate = DateTime.Now.AddDays(-1), IsActive = false };
        order.Products.Add(p1);
        order.Products.Add(p3);
        orders.Add(order);
        p1.IsSold = true;
        p3.IsSold = true;
    }
}

public class MenuUI
{
    private ProductService service = new ProductService();

    public void Run()
    {
        service.SeedExampleData();
        while (true)
        {
            Console.WriteLine("\nRestaurant menu manager");
            Console.WriteLine("1 Add dish");
            Console.WriteLine("2 Edit dish");
            Console.WriteLine("3 Delete dish");
            Console.WriteLine("4 Show all dishes");
            Console.WriteLine("5 Create order");
            Console.WriteLine("6 Close order");
            Console.WriteLine("7 Show sales statistics");
            Console.WriteLine("0 Exit");
            Console.Write("Choice: ");
            var choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1":
                    service.AddProduct();
                    break;
                case "2":
                    service.EditProduct();
                    break;
                case "3":
                    service.DeleteProduct();
                    break;
                case "4":
                    service.ShowAllProducts();
                    break;
                case "5":
                    service.CreateOrder();
                    break;
                case "6":
                    service.CloseOrder();
                    break;
                case "7":
                    service.ShowSalesStatistics();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice");
                    break;
            }
        }
    }
}

public class Program
{
    public static void Main()
    {
        var menu = new MenuUI();
        menu.Run();
    }
}
