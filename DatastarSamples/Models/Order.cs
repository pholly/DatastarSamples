using Bogus;

namespace DatastarSamples.Models;

public class Order
{
    public static string[] Fruits = new[] {"apple", "banana", "orange", "strawberry", "kiwi"};
    public static IEnumerable<Order> SampleOrders;

    static Order()
    {
        var orderIds = 0;
        var testOrders = new Faker<Order>()
            //Ensure all properties have rules. By default, StrictMode is false
            //Set a global policy by using Faker.DefaultStrictMode if you prefer.
            .StrictMode(true)
            //OrderId is deterministic
            .RuleFor(o => o.OrderId, f => orderIds++)
            //Pick some fruit from a basket
            .RuleFor(o => o.Item, f => f.PickRandom(Fruits))
            //A random quantity from 1 to 10
            .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
            //A nullable int? with 80% probability of being null.
            //The .OrNull extension is in the Bogus.Extensions namespace.
            .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f));

        SampleOrders = testOrders.Generate(100);
    }
    
    public int OrderId { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public int? LotNumber { get; set; }
    
    
}