namespace APIx.Config;

public class QueueConfig
{
    public required string HostName { get; set; }
    public required string Queue { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string VirtualHost { get; set; }

    public int MaxChannelCount { get; set; } =  Environment.ProcessorCount * 2;
}