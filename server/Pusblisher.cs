using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PusherServer;
#pragma warning disable 1998

public class Publisher
{
    static readonly PusherOptions options = new()
    {
        Cluster = "eu",
        Encrypted = true
    };

    // Add appId, appKey and appSecret for your account
    static readonly Pusher pusher = new(
      < appId >,
      < appKey >,
      < appSecret >,
      options);

    public async Task<List<HttpStatusCode>> SendMessageAsync(string[] channels, string eventName, string message)
    {
        var result = new List<HttpStatusCode>();
        int chunkSize = 100;
        int totalChunks = (int)Math.Ceiling((double)channels.Length / chunkSize);
        for (int i = 0; i < totalChunks; i++)
        {
            var channelsChunk = channels.Skip(i * chunkSize).Take(chunkSize).ToArray();
            var chunkResult = await pusher.TriggerAsync(channelsChunk, eventName, message);
            result.Add(chunkResult.StatusCode);
        }

        return result;
    }

    public async Task<List<HttpStatusCode>> SendBatchMessageAsync(Event[] events)
    {
        var result = new List<HttpStatusCode>();
        int chunkSize = 10;
        int totalChunks = (int)Math.Ceiling((double)events.Length / chunkSize);
        for (int i = 0; i < totalChunks; i++)
        {
            var eventsChunk = events.Skip(i * chunkSize).Take(chunkSize).ToArray();
            var chunkResult = await pusher.TriggerAsync(eventsChunk);
            result.Add(chunkResult.StatusCode);
        }

        return result;
    }

    public async Task<List<HttpStatusCode>> SendParallelBatchMessageAsync(Event[] events)
    {
        var result = new List<HttpStatusCode>();
        int chunkSize = 10;
        int totalChunks = (int)Math.Ceiling((double)events.Length / chunkSize);
        Parallel.For(0, totalChunks, async i =>
        {
            var eventsChunk = events.Skip(i * chunkSize).Take(chunkSize).ToArray();
            var chunkResult = await pusher.TriggerAsync(eventsChunk);
            lock (result)
            {
                result.Add(chunkResult.StatusCode);
            }
        });

        return result;
    }
}