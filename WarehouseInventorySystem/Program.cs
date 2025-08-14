using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseInventorySystem
{
    // Marker Interface for Inventory Items
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // Electronic Item Product Class
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Brand = brand;
            WarrantyMonths = warrantyMonths;
        }

        public override string ToString()
        {
            return $"Electronic: {Name} (ID: {Id}, Brand: {Brand}, Quantity: {Quantity}, Warranty: {WarrantyMonths} months)";
        }
    }

    // Grocery Item Product Class
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            ExpiryDate = expiryDate;
        }

        public override string ToString()
        {
            return $"Grocery: {Name} (ID: {Id}, Quantity: {Quantity}, Expires: {ExpiryDate:yyyy-MM-dd})";
        }
    }

    // Custom Exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // Generic Inventory Repository
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private Dictionary<int, T> items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Item with ID {item.Id} already exists in inventory.");
            }
            items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
            }
            return items[id];
        }

        public void RemoveItem(int id)
        {
            if (!items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
            }
            items.Remove(id);
        }

        public List<T> GetAllItems()
        {
            return items.Values.ToList();
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException($"Quantity cannot be negative. Provided: {newQuantity}");
            }

            if (!items.ContainsKey(id))
            {
                throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
            }

            items[id].Quantity = newQuantity;
        }
    }

    // Warehouse Manager Class
    public class WareHouseManager
    {
        private InventoryRepository<ElectronicItem> _electronics;
        private InventoryRepository<GroceryItem> _groceries;

        public WareHouseManager()
        {
            _electronics = new InventoryRepository<ElectronicItem>();
            _groceries = new InventoryRepository<GroceryItem>();
        }

        public void SeedData()
        {
            try
            {
                // Add Electronic Items
                _electronics.AddItem(new ElectronicItem(1001, "Laptop", 15, "Dell", 24));
                _electronics.AddItem(new ElectronicItem(1002, "Smartphone", 50, "Samsung", 12));
                _electronics.AddItem(new ElectronicItem(1003, "Tablet", 25, "Apple", 12));

                // Add Grocery Items
                _groceries.AddItem(new GroceryItem(2001, "Milk", 100, DateTime.Now.AddDays(7)));
                _groceries.AddItem(new GroceryItem(2002, "Bread", 75, DateTime.Now.AddDays(3)));
                _groceries.AddItem(new GroceryItem(2003, "Apples", 200, DateTime.Now.AddDays(10)));

                Console.WriteLine("✓ Sample data seeded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.Message}");
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            try
            {
                var items = repo.GetAllItems();
                if (items.Count == 0)
                {
                    Console.WriteLine("No items found in inventory.");
                    return;
                }

                foreach (var item in items)
                {
                    Console.WriteLine($"  {item}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving items: {ex.Message}");
            }
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                int newQuantity = item.Quantity + quantity;
                repo.UpdateQuantity(id, newQuantity);
                Console.WriteLine($"✓ Stock increased for {item.Name}. New quantity: {newQuantity}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✗ Error increasing stock: {ex.Message}");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"✗ Error increasing stock: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Unexpected error increasing stock: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.RemoveItem(id);
                Console.WriteLine($"✓ Item removed: {item.Name} (ID: {id})");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✗ Error removing item: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Unexpected error removing item: {ex.Message}");
            }
        }

        // Public accessors for repositories
        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Warehouse Inventory Management System ===\n");

            // Step i: Instantiate WareHouseManager
            var warehouse = new WareHouseManager();

            // Step ii: Call SeedData()
            Console.WriteLine("1. Seeding Initial Data:");
            warehouse.SeedData();
            Console.WriteLine();

            // Step iii: Print all grocery items
            Console.WriteLine("2. All Grocery Items:");
            warehouse.PrintAllItems(warehouse.Groceries);
            Console.WriteLine();

            // Step iv: Print all electronic items
            Console.WriteLine("3. All Electronic Items:");
            warehouse.PrintAllItems(warehouse.Electronics);
            Console.WriteLine();

            // Step v: Error handling demonstrations
            Console.WriteLine("4. Exception Handling Demonstrations:\n");

            // Try to add a duplicate item
            Console.WriteLine("a) Attempting to add duplicate item:");
            try
            {
                warehouse.Electronics.AddItem(new ElectronicItem(1001, "Duplicate Laptop", 10, "HP", 12));
                Console.WriteLine("✓ Item added successfully");
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            Console.WriteLine();

            // Try to remove a non-existent item
            Console.WriteLine("b) Attempting to remove non-existent item:");
            warehouse.RemoveItemById(warehouse.Groceries, 9999);
            Console.WriteLine();

            // Try to update with invalid quantity
            Console.WriteLine("c) Attempting to update with invalid quantity:");
            try
            {
                warehouse.Groceries.UpdateQuantity(2001, -50);
                Console.WriteLine("✓ Quantity updated successfully");
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine($"✗ {ex.Message}");
            }
            Console.WriteLine();

            // Additional demonstrations
            Console.WriteLine("5. Successful Operations:");
            
            // Increase stock for existing item
            warehouse.IncreaseStock(warehouse.Electronics, 1002, 25);
            
            // Remove existing item
            warehouse.RemoveItemById(warehouse.Groceries, 2003);
            
            // Update quantity with valid value
            try
            {
                warehouse.Electronics.UpdateQuantity(1001, 20);
                Console.WriteLine("✓ Laptop quantity updated to 20");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }

            Console.WriteLine("\n=== Final Inventory Status ===");
            Console.WriteLine("\nElectronics:");
            warehouse.PrintAllItems(warehouse.Electronics);
            
            Console.WriteLine("\nGroceries:");
            warehouse.PrintAllItems(warehouse.Groceries);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}