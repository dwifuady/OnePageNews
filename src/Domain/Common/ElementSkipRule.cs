namespace Domain.Common;

public class ElementSkipRule
{
    public ElementSkipRuleEnum RuleEnum { get; }
    public string RuleValue { get; }

    public ElementSkipRule(ElementSkipRuleEnum ruleEnum, string ruleValue)
    {
        RuleEnum = ruleEnum;
        RuleValue = ruleValue;
    }
}

public enum ElementSkipRuleEnum
{
    StartsWith,
    Contain
}