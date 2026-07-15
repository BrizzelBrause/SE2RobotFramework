namespace SE2RobotFramework.Configuration;

public readonly record struct RuntimeConfigurationApplyResult<TConfiguration>(
    bool IsSuccess,
    TConfiguration? Configuration,
    RuntimeConfigurationError Error,
    string? ErrorMessage)
    where TConfiguration : class;
