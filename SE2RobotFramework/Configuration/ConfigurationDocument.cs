namespace SE2RobotFramework.Configuration;

public class ConfigurationDocument<TConfiguration>
{
    public int SchemaVersion { get; init; }

    public TConfiguration? Configuration { get; init; }
}
