using Microsoft.Extensions.Caching.Memory;
using Xunit;

public class CachedProductServiceTests
{
    private static (CachedProductService Service, ProductRepository Repository) Make()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var repository = new ProductRepository();
        return (new CachedProductService(cache, repository), repository);
    }

    [Fact]
    public void GetAllProducts_FirstCall_HitsRepository()
    {
        var (service, repository) = Make();

        var products = service.GetAllProducts();

        Assert.Equal(1, repository.CallCount);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public void GetAllProducts_SecondCall_UsesCacheNotRepository()
    {
        var (service, repository) = Make();

        service.GetAllProducts();
        service.GetAllProducts();
        service.GetAllProducts();

        Assert.Equal(1, repository.CallCount);
    }

    [Fact]
    public void InvalidateCache_ForcesNextCallToHitRepositoryAgain()
    {
        var (service, repository) = Make();

        service.GetAllProducts();
        service.InvalidateCache();
        service.GetAllProducts();

        Assert.Equal(2, repository.CallCount);
    }

    [Fact]
    public void GetAllProducts_ReturnsCorrectData()
    {
        var (service, _) = Make();

        var products = service.GetAllProducts();

        Assert.Contains(products, p => p.Name == "Widget");
        Assert.Contains(products, p => p.Name == "Gadget");
    }
}
