using Xunit;

public class PayrollTests
{
    [Fact]
    public void Invoice_ImplementsIPayable()
    {
        Assert.IsAssignableFrom<IPayable>(new Invoice { ClientName = "Acme", Amount = 100 });
    }

    [Fact]
    public void Employee_ImplementsIPayable()
    {
        Assert.IsAssignableFrom<IPayable>(new Employee { Name = "Sara", Amount = 100 });
    }

    [Fact]
    public void Invoice_Describe()
    {
        var i = new Invoice { ClientName = "Acme Co", Amount = 500 };
        Assert.Equal("Invoice for Acme Co: 500", i.Describe());
    }

    [Fact]
    public void Employee_Describe()
    {
        var e = new Employee { Name = "Sara", Amount = 3000 };
        Assert.Equal("Sara's pay: 3000", e.Describe());
    }

    [Fact]
    public void Total_SumsMixedList()
    {
        List<IPayable> payables = new List<IPayable>
        {
            new Invoice { ClientName = "Acme", Amount = 500 },
            new Employee { Name = "Sara", Amount = 3000 },
            new Employee { Name = "Omar", Amount = 2500 },
        };
        Assert.Equal(6000, Payroll.Total(payables));
    }

    [Fact]
    public void Total_EmptyList_IsZero()
    {
        Assert.Equal(0, Payroll.Total(new List<IPayable>()));
    }
}
