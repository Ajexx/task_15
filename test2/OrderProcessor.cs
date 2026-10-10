using System.Diagnostics;

public class OrderProcessor
{
    public volatile bool IsCancelled;
    public void Cancel()
    {
        IsCancelled = true;
    }
    public void Step()
    {
        if (IsCancelled)
        {
            throw new OperationCanceledException("Processing cancelled.");
        } 
        Thread.Sleep(Random.Shared.Next(100, 500));
    }

    public void ProcessOrder(Order order)
    {
        order.Status = OrderStatus.Processing;
        Step();
        Step();
        Step();
        if (Random.Shared.Next(0, 10) < 1)
        {
            throw new Exception("Order processing failed.");
        }
        Step();
        order.Status = OrderStatus.Completed;
    }

    public void Handle(Order order, Stats stats)
    {
        if (IsCancelled)
        {
            order.Status = OrderStatus.Cancelled;
            return;
        }
        stats.IncrementProcessingOrders();

        try
        {
            ProcessOrder(order);
            stats.IncrementSuccessfulOrders();
        }
        catch(OperationCanceledException)
        {
            order.Status = OrderStatus.Cancelled;
            Console.WriteLine($"Order {order.Id} cancelled.");
        }
        catch (Exception ex)
        {
            order.Status = OrderStatus.Failed;
            stats.IncrementFailedOrders(order.Id);
            Console.WriteLine($"Order {order.Id} failed: {ex.Message}");
        }
        finally
        {
            stats.DecrementProcessingOrders();
        }
    }

    public void ProcessOrdersSequentially(List<Order> orders, Stats stats)
    {
        var sw = Stopwatch.StartNew();
        foreach (var order in orders)
        {
            Handle(order, stats);
        }
        sw.Stop();
        Console.WriteLine($"Sequential processing time: {sw.ElapsedMilliseconds} ms");
    }

    public void ProcessOrdersInParallel(List<Order> orders, Stats stats)
    {
        var sw = Stopwatch.StartNew();
        Parallel.ForEach(orders, new ParallelOptions { MaxDegreeOfParallelism = 5 }, order =>
        {
            Handle(order, stats);
        });
        sw.Stop();
        Console.WriteLine($"Parallel processing time: {sw.ElapsedMilliseconds} ms");
    }
}

// "В конце вывести результаты и объяснить, почему время выполнения отличается."
// Потому что в режиме последовательной обработки каждый заказ обрабатывается один за другим,
// что приводит к увеличению общего времени выполнения.
// В режиме параллельной обработки несколько заказов обрабатываются одновременно (до 5),
// это позволяет значительно сократить время выполнения,
// так как операции выполняются параллельно, а не последовательно.