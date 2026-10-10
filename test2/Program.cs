
public class Program
{
    public static List<Order> CreateOrders() =>
        Enumerable
        .Range(1, 100)
        .Select(i => new Order(i, $"Client {i}", 10m * i))
        .ToList();

    public static void PrintStatistics(string mode, Stats stats, int totalOrders)
    {
        Console.WriteLine($"\n========== ORDER PROCESSING ({mode}) ==========");
        Console.WriteLine($"Total orders:        {totalOrders}");
        Console.WriteLine($"Successful orders:   {stats.SuccessfulOrdersCount}");
        Console.WriteLine($"Failed orders:       {stats.FailedOrdersCount}");
        Console.WriteLine($"Processing orders:   {stats.ProcessingOrdersCount}");
        Console.WriteLine($"Processing time:     {stats.Elapsed.TotalSeconds:F2} sec");
        Console.WriteLine($"Failed IDs:          {string.Join(", ", stats.FailedOrderIds.OrderBy(id => id))}");
        Console.WriteLine("====================================================================================");
    }
    public static void Main(string[] args)
    {   
        var processor = new OrderProcessor();

        new Thread(() => 
        { 
            Console.ReadKey(true);
            processor.Cancel(); 
        }) 
        { 
            IsBackground = true 
        }
        .Start();
        Console.WriteLine("Enter any key to cancel...\n");

        var seqOrders = CreateOrders();
        var seqStats = new Stats();
        processor.ProcessOrdersSequentially(seqOrders, seqStats);
        
        PrintStatistics("SEQUENTIAL", seqStats, seqOrders.Count);
        if (processor.IsCancelled)
        {
            Console.WriteLine("Processing has been disabled by the user.");
            return;
        }

        //

        var parOrders = CreateOrders();
        var parStats = new Stats();
        processor.ProcessOrdersInParallel(parOrders, parStats);
        PrintStatistics("PARALLEL (max 5)", parStats, parOrders.Count);

        if (processor.IsCancelled)
        {
            Console.WriteLine("Processing has been disabled by the user.");
            return;
        }
    }
}

// "В конце вывести результаты и объяснить, почему время выполнения отличается."
// Потому что в режиме последовательной обработки каждый заказ обрабатывается один за другим,
// что приводит к увеличению общего времени выполнения.
// В режиме параллельной обработки несколько заказов обрабатываются одновременно (до 5),
// это позволяет значительно сократить время выполнения,
// так как операции выполняются параллельно, а не последовательно.