using System.ComponentModel;

namespace Infrastructure.Logging;

public sealed class LogConfiguration
{
    public LogTechnology Using { get; set; }
    public LogType To { get; set; }

    public LogConfiguration()
    {
        
    }
    
    public LogConfiguration(LogTechnology @using, LogType to)
    {
        Using = @using;
        To = to;
    }
}

public enum LogTechnology
{
    [Description("Serilog")]
    Serilog
}
public enum LogType
{
    [Description("File")]
    File,
    
    [Description("Elasticsearch")]
    Elasticsearch
}