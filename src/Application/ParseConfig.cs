using System.Text.Json.Serialization;
using Domain.Common;

namespace Application;

public class Configurations
{
    public List<ParseConfig>? ParseConfigs { get; set; }
}

public class ParseConfig
{
    public string Provider { get; init; } = null!;
    public string TitlePath { get; init; } = null!;
    public string ContentPath { get; init; } = null!;
    public bool HasSinglePageUrl { get; init; }
    public bool HasAllPageUrl { get; init; }
    public string? SinglePagePath { get; init; }
    public string? AllPagePath { get; init; }
    public List<ElementRule>? ElementRules { get; init; }
    
    [JsonIgnore]
    public ProviderEnum ProviderEnum => (ProviderEnum)Enum.Parse(typeof(ProviderEnum), Provider);
}
