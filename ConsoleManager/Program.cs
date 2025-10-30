using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DataAccess.Entities;
using DataAccess;

// Припускаємо, що ці класи вже створені першим учасником
// public class MenuItem { ... }
// public class Category { ... }
// public class RestaurantDbContext : DbContext { ... }

class MenuManager
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        using var db = new RestaurantDbContext();

        while (true)
        {
            Console.WriteLine("\n=== МЕНЕДЖЕР МЕНЮ РЕСТОРАНУ ===");
            Console.WriteLine("1. Додати страву");
            Console.WriteLine("2. Редагувати страву");
            Console.WriteLine("3. Видалити страву");
            Console.WriteLine("4. Показати всі страви");
            Console.WriteLine("5. Показати страви за категорією");
            Console.WriteLine("0. Вихід");
            Console.Write("Ваш вибір: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddDish(db); break;
                case "2": EditDish(db); break;
                case "3": DeleteDish(db); break;
                case "4": ShowAllDishes(db); break;
                case "5": ShowByCategory(db); break;
                case "0":
                    Console.WriteLine("До побачення!");
                    return;
                default:
                    Console.WriteLine("❌ Невірний вибір. Спробуйте ще раз.");
                    break;
            }
        }
    }

    static void AddDish(RestaurantDbContext db)
    {
        Console.WriteLine("\n--- ДОДАТИ НОВУ СТРАВУ ---");

        Console.Write("Назва страви: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Опис: ");
        string description = Console.ReadLine() ?? "";

        Console.Write("Ціна (грн): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("❌ Невірний формат ціни!");
            return;
        }

        Console.Write("Алергени (через кому): ");
        string allergens = Console.ReadLine() ?? "";

        Console.Write("Категорія: ");
        string categoryName = Console.ReadLine() ?? "";
        var category = db.Categories.FirstOrDefault(c => c.Name == categoryName);
        if (category == null)
        {
            category = new Category { Name = categoryName };
            db.Categories.Add(category);
        }

        var dish = new Dish
        {
            Name = name,
            Description = description,
            Price = price,
            Allergens = allergens,
            Category = category
        };

        db.Dishes.Add(dish);
        db.SaveChanges();

        Console.WriteLine($"✅ Страву '{name}' успішно додано!");
    }

    static void EditDish(RestaurantDbContext db)
    {
        ShowAllDishes(db);

        Console.Write("\nВведіть ID страви для редагування: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ Невірний ID!");
            return;
        }
        //rrrrrrrrrrrrr
        //rrrrrrrrrrrrr
        //rrrrrrrrrrrrr
        //rrrrrrrrrrrrr
        //rrrrrrrrrrrrr
        //rrrrrrrrrrrrr
        //rrrrrrrrrrrrr

        var dish = db.Dishes.Include(d => d.Category).FirstOrDefault(d => d.Id == id);
        if (dish == null)
        {
            Console.WriteLine("❌ Страву не знайдено!");
            return;
        }

        Console.WriteLine($"\nРедагування: {dish.Name}");
        Console.WriteLine("(Натисніть Enter, щоб залишити без змін)\n");

        Console.Write($"Назва [{dish.Name}]: ");
        var newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName)) dish.Name = newName;

        Console.Write($"Опис [{dish.Description}]: ");
        var newDesc = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newDesc)) dish.Description = newDesc;

        Console.Write($"Ціна [{dish.Price}]: ");
        if (decimal.TryParse(Console.ReadLine(), out var newPrice))
            dish.Price = newPrice;

        Console.Write($"Алергени [{dish.Allergens}]: ");
        var newAllergens = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newAllergens)) dish.Allergens = newAllergens;

        Console.Write($"Категорія [{dish.Category?.Name}]: ");
        var newCategoryName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newCategoryName))
        {
            var category = db.Categories.FirstOrDefault(c => c.Name == newCategoryName);
            if (category == null)
            {
                category = new Category { Name = newCategoryName };
                db.Categories.Add(category);
            }
            dish.Category = category;
        }

        db.SaveChanges();
        Console.WriteLine("✅ Страву успішно оновлено!");
    }

    static void DeleteDish(RestaurantDbContext db)
    {
        ShowAllDishes(db);

        Console.Write("\nВведіть ID страви для видалення: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ Невірний ID!");
            return;
        }

        var dish = db.Dishes.Find(id);
        if (dish == null)
        {
            Console.WriteLine("❌ Страву не знайдено!");
            return;
        }

        Console.Write($"⚠️ Ви впевнені, що хочете видалити '{dish.Name}'? (так/ні): ");
        var confirm = Console.ReadLine()?.ToLower();

        if (confirm == "так" || confirm == "yes")
        {
            db.Dishes.Remove(dish);
            db.SaveChanges();
            Console.WriteLine("✅ Страву видалено!");
        }
        else
        {
            Console.WriteLine("❌ Видалення скасовано.");
        }
    }

    static void ShowAllDishes(RestaurantDbContext db)
    {
        var dishes = db.Dishes.Include(d => d.Category).ToList();

        if (dishes.Count == 0)
        {
            Console.WriteLine("\n📝 Меню порожнє. Додайте страви!");
            return;
        }

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║            📋 ВСІ СТРАВИ МЕНЮ                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        foreach (var dish in dishes)
        {
            Console.WriteLine($"\n🆔 ID: {dish.Id}");
            Console.WriteLine($"📌 Назва: {dish.Name}");
            Console.WriteLine($"💰 Ціна: {dish.Price} ₴");
            Console.WriteLine($"📂 Категорія: {dish.Category?.Name ?? "Без категорії"}");
            Console.WriteLine($"📝 Опис: {dish.Description}");
            if (!string.IsNullOrEmpty(dish.Allergens))
                Console.WriteLine($"⚠️ Алергени: {dish.Allergens}");
            Console.WriteLine("───────────────────────────────────────────────");
        }
    }

    static void ShowByCategory(RestaurantDbContext db)
    {
        Console.Write("\nВведіть назву категорії: ");
        string categoryName = Console.ReadLine() ?? "";

        var dishes = db.Dishes
            .Include(d => d.Category)
            .Where(d => d.Category != null && d.Category.Name == categoryName)
            .ToList();

        if (dishes.Count == 0)
        {
            Console.WriteLine($"\n❌ У категорії '{categoryName}' немає страв.");
            return;
        }

        Console.WriteLine($"\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine($"║    📂 КАТЕГОРІЯ: {categoryName.ToUpper().PadRight(28)} ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        foreach (var dish in dishes)
        {
            Console.WriteLine($"\n🍽️ {dish.Name} — {dish.Price} ₴");
            Console.WriteLine($"   {dish.Description}");
            if (!string.IsNullOrEmpty(dish.Allergens))
                Console.WriteLine($"   ⚠️ Алергени: {dish.Allergens}");
        }
    }
}