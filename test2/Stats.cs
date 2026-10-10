using System.Collections.Concurrent;

public class Stats
{
    public int SuccessfulOrders;
    public int FailedOrders;
    public int ProcessingOrders;
    public TimeSpan Elapsed { get; set; }

    public ConcurrentBag<int> FailedOrderIds { get; set; } = new ConcurrentBag<int>();
    
    public int SuccessfulOrdersCount => SuccessfulOrders;
    public int FailedOrdersCount => FailedOrders;
    public int ProcessingOrdersCount => ProcessingOrders;

    public void IncrementProcessingOrders()
        => Interlocked.Increment(ref ProcessingOrders);
    public void DecrementProcessingOrders()
        => Interlocked.Decrement(ref ProcessingOrders);
    public void IncrementSuccessfulOrders() 
        => Interlocked.Increment(ref SuccessfulOrders);

    public void IncrementFailedOrders(int orderId)
    {
        Interlocked.Increment(ref FailedOrders);
        FailedOrderIds.Add(orderId);
    }
}

// "В конце вывести результаты и объяснить, почему время выполнения отличается."
// Потому что в режиме последовательной обработки каждый заказ обрабатывается один за другим,
// что приводит к увеличению общего времени выполнения.
// В режиме параллельной обработки несколько заказов обрабатываются одновременно (до 5),
// это позволяет значительно сократить время выполнения,
// так как операции выполняются параллельно, а не последовательно.