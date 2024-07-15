namespace Indra.Astra.Rules {

  public abstract partial class Rule {
    /// <summary>
    /// If this rule is a sub-rule of a larger rule, this will be the parent rule.
    /// </summary>
    public interface IPart {
      public Rule Parent { get; }
    }
  }
}