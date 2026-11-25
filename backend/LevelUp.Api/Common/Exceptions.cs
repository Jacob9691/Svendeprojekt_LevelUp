namespace LevelUp.Api.Common;

/// <summary>Ressourcen findes ikke (eller tilhører en anden bruger). Bliver til 404.</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Handlingen strider mod en forretningsregel, f.eks. for få points. Bliver til 409.</summary>
public class BusinessRuleException(string message) : Exception(message);
