using Microsoft.EntityFrameworkCore;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Order> Orders { get; set; } = [];
}

public class Order
{
    public int Id { get; set; }
    public double Total { get; set; }
    public int CustomerId { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
}

public class ShopService
{
    private readonly AppDbContext _context;

    public ShopService(AppDbContext context) => _context = context;

    public async Task<int> DeleteCustomerAsync(int id)
    {
        var customer = await _context.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);
        if (customer is null) return -1;
        int orders = customer.Orders.Count;
        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return orders;
    }

    public async Task<List<string>> BigSpendersAsync(double limit)
    {
        return await _context.Customers.Where(c => c.Orders.Sum(o => o.Total) > limit)
            .OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();
    }
}
