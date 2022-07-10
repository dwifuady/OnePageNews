using Domain.Common;
using System.Text.Json;

namespace Application;

public static class ConfigLoader
{
    public static IEnumerable<ParseConfig>? LoadConfig()
    {
        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        
        using var reader = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json"));
        var json = reader.ReadToEnd();
        var configs = JsonSerializer.Deserialize<Configurations>(json, serializeOptions);

        return configs?.ParseConfigs;
    }

    private static ParseConfig LoadDetikConfig()
    {
        return new ParseConfig()
        {
            Provider = ProviderEnum.Detik.ToString(),
            TitlePath = "//h1[@class='detail__title']",
            ContentPath = "//*[@class='detail__body-text itp_bodycontent']",
            AllPagePath = "//*[@class='detail__anchor-numb ']",
            HasSinglePageUrl = false,
            ElementRules = new List<ElementRule>
            {
                new(ElementRuleEnum.StartsWith, "<strong>", ElementType.Skip),
                new(ElementRuleEnum.StartsWith, "<em>", ElementType.Skip),
                new(ElementRuleEnum.Contain, "20detik", ElementType.Skip),
                new(ElementRuleEnum.Contain, "ADVERTISEMENT", ElementType.Skip),
                new(ElementRuleEnum.Contain, "SCROLL TO RESUME CONTENT", ElementType.Skip)
            }
        };
    }
    
    private static ParseConfig LoadKompasConfig()
    {
        return new ParseConfig()
        {
            Provider = ProviderEnum.Kompas.ToString(),
            TitlePath = "//h1[@class='read__title']",
            ContentPath = "//*[@class='read__content']",
            SinglePagePath = "//*[@class='paging__link paging__link--show']",
            HasSinglePageUrl = true,
            ElementRules = new List<ElementRule>
            {
                new(ElementRuleEnum.StartsWith, "<strong>", ElementType.Skip),
                new(ElementRuleEnum.StartsWith, "<em>", ElementType.Skip),
                new(ElementRuleEnum.Contain, "Tulis komentarmu dengan tagar", ElementType.End)
            }
        };
    }
}