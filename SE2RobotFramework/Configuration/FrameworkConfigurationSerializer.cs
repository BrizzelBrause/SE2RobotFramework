using System.Text.Json;
using System.Text.Json.Serialization;

namespace SE2RobotFramework.Configuration;

public class FrameworkConfigurationSerializer
{
    public const int CurrentSchemaVersion = 1;

    private readonly JsonSerializerOptions _options;

    public FrameworkConfigurationSerializer()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        _options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public string SerializeSolarArray(SolarArrayConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();
        return Serialize(configuration);
    }

    public SolarArrayConfiguration DeserializeSolarArray(string json)
    {
        SolarArrayConfiguration configuration =
            Deserialize<SolarArrayConfiguration>(json);
        configuration.Validate();
        return configuration;
    }

    public string SerializeDrillArm(DrillArmConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Validate();
        return Serialize(configuration);
    }

    public DrillArmConfiguration DeserializeDrillArm(string json)
    {
        DrillArmConfiguration configuration =
            Deserialize<DrillArmConfiguration>(json);
        configuration.Validate();
        return configuration;
    }

    private string Serialize<TConfiguration>(TConfiguration configuration)
    {
        ConfigurationDocument<TConfiguration> document = new()
        {
            SchemaVersion = CurrentSchemaVersion,
            Configuration = configuration
        };

        return JsonSerializer.Serialize(document, _options);
    }

    private TConfiguration Deserialize<TConfiguration>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException(
                "Configuration JSON cannot be empty.",
                nameof(json));
        }

        ConfigurationDocument<TConfiguration>? document =
            JsonSerializer.Deserialize<ConfigurationDocument<TConfiguration>>(
                json,
                _options);

        if (document is null)
        {
            throw new JsonException("The configuration document is missing.");
        }

        if (document.SchemaVersion != CurrentSchemaVersion)
        {
            throw new NotSupportedException(
                $"Configuration schema version {document.SchemaVersion} is not supported.");
        }

        return document.Configuration ??
            throw new JsonException("The configuration payload is missing.");
    }
}
