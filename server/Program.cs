using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using PusherServer;




int[] counts = new[] { 1, 10, 100, 1000, 10000 };
string method = "event";  // event, batch-event, parallel-batch-event


Console.WriteLine("Start!");
foreach (var count in counts)
{
    Console.WriteLine($"Count = {count}; Method = {method}");
    await Test_PusherTriggers(count, method);
    Console.WriteLine("***********");
}
Console.WriteLine("End!");





static async Task Test_PusherTriggers(int count, string method)
{
    Stopwatch stopwatch = new();

    stopwatch.Start();
    switch (method)
    {
        case "event":
            // Send to event endpoint
            await SendEvent(count);
            break;
        case "batch-event":
            // Send to batch_event endpoint
            await SendBatchEvent(count);
            break;
        case "parallel-batch-event":
            // Send to batch_event endpoint with a parallel for loop
            await SendBatchEvent(count, true);
            break;
        default:
            break;
    }
    stopwatch.Stop();

    TimeSpan elapsedTime = stopwatch.Elapsed;
    string formattedElapsedTime = FormatElapsedTime(elapsedTime);
    Console.WriteLine($"Elapsed Time: {formattedElapsedTime}");
}

static async Task SendEvent(int count)
{
    var publisher = new Publisher();
    var channels = new List<string>();
    for (int i = 1; i <= count; i++)
    {
        channels.Add(string.Format("channel-{0}", i));
    }
    var result = await publisher.SendMessageAsync(channels.ToArray(), "event", "sample message");
    // Console.WriteLine("### " + string.Join(", ", result));
}

static async Task SendBatchEvent(int count, bool parallel = false)
{
    var publisher = new Publisher();
    var events = new List<Event>();
    for (int i = 1; i <= count; i++)
    {
        events.Add(new Event
        {
            Channel = string.Format("batch-channel-{0}", i),
            EventName = "batch-event",
            Data = new
            {
                Channel = string.Format("batch-channel-{0}", i),
                Event = "batch-event",
                Message = string.Format("{0} - sample message", i)
            }
        });
    }
    List<HttpStatusCode> batchResult;
    if (!parallel)
    {
        batchResult = await publisher.SendBatchMessageAsync(events.ToArray());
    }
    else
    {
        batchResult = await publisher.SendParallelBatchMessageAsync(events.ToArray());
    }
    // Console.WriteLine("### " + string.Join(", ", batchResult));
}

static string FormatElapsedTime(TimeSpan elapsedTime)
{
    return $"{elapsedTime.Minutes:D2}:{elapsedTime.Seconds:D2}.{elapsedTime.Milliseconds:D3}";
}

