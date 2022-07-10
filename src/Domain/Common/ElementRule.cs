namespace Domain.Common;

public class ElementRule
{
    public ElementRuleEnum RuleEnum { get; }
    public string RuleValue { get; }
    public ElementType ElementType { get; }

    public ElementRule(ElementRuleEnum ruleEnum, string ruleValue, ElementType elementType)
    {
        RuleEnum = ruleEnum;
        RuleValue = ruleValue;
        ElementType = elementType;
    }
}

public enum ElementRuleEnum
{
    StartsWith,
    Contain
}

public enum ElementType
{
    Skip,
    End
}