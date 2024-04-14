using Common.EventBus;
using DotNetCore.CAP;
using MediaEncoder.Domain.Entities;
using MediaEncoder.Infrastructure;
using Microsoft.EntityFrameworkCore;

using Dynamic.Json;

namespace MediaEncoder.WebAPI.EventHandlers;
public class MediaEncodingCreatedEvent
{
    public Guid MediaId { get; set; }
    public string MediaUrl { get; set; }
    public string OutputFormat { get; set; }
    public string SourceSystem { get; set; }
}
public class MediaEncodingCreatedHandler: ICapSubscribe, ISubscriberService<MediaEncodingCreatedEvent>
{
    private readonly ICapPublisher capPublisher;
    private readonly MediaEncoderDbContext mediaEncoderDbContext;
    private readonly ILogger<MediaEncodingCreatedHandler> logger;

    public MediaEncodingCreatedHandler(ICapPublisher capPublisher, MediaEncoderDbContext mediaEncoderDbContext, ILogger<MediaEncodingCreatedHandler> logger)
    {
        this.capPublisher = capPublisher;
        this.mediaEncoderDbContext = mediaEncoderDbContext;
        this.logger = logger;
    }

    [CapSubscribe("MediaEncoding.Created")]
    public async Task  HandleSubscriber(MediaEncodingCreatedEvent eventData)
    {
        Guid mediaId = eventData.MediaId;
        Uri mediaUrl = new Uri(eventData.MediaUrl);
        string sourceSystem = eventData.SourceSystem;
        // 获取 mediaUrl 一个字符串数，每个段由斜杠（/）分隔
        string fileName = mediaUrl.Segments.Last();
        string outputFormat = eventData.OutputFormat;
        //保证幂等性，如果这个路径对应的操作已经存在，则直接返回
        bool exists = await mediaEncoderDbContext.EncodingItems
            .AnyAsync(e => e.SourceUrl == mediaUrl && e.OutputFormat == outputFormat);
        if (exists)
        {
            return;
        }

        //把任务插入数据库，也可以看作是一种事件，不一定非要放到MQ中才叫事件
        //没有通过领域事件执行，因为如果一下子来很多任务，领域事件就会并发转码，而这种方式则会一个个的转码
        //直接用另一端传来的MediaId作为EncodingItem的主键
        var encodeItem = EncodingItem.Create(mediaId, fileName, mediaUrl, outputFormat, sourceSystem);
        mediaEncoderDbContext.Add(encodeItem);
        await mediaEncoderDbContext.SaveChangesAsync();
    }

}
