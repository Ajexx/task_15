public class Order
{
    public int Id { get; set; }
    public string? clientName { get; set; }
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; }

    public Order(int id, string clientName, decimal amount)
    {
        Id = id;
        this.clientName = clientName;
        Amount = amount;
        Status = OrderStatus.New;
    }
}

// "В конце вывести результаты и объяснить, почему время выполнения отличается."
// Потому что в режиме последовательной обработки каждый заказ обрабатывается один за другим,
// что приводит к увеличению общего времени выполнения.
// В режиме параллельной обработки несколько заказов обрабатываются одновременно (до 5),
// это позволяет значительно сократить время выполнения,
// так как операции выполняются параллельно, а не последовательно.