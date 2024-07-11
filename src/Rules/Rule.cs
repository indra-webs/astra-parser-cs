namespace Indra.Astra.Rules {

  public abstract partial class Rule
  : IRule<Rule> {

    #region Private Fields

    private static readonly Lazy<Lexer> _lexer
        = new();

    #endregion

    /// <summary>
    /// If this rule not generate a complete expression, but instead
    ///   should be used as part of a the larger, containing, named rule.
    /// </summary>
    public virtual bool IsPartial
      => true;

    protected Rule() { }

    public abstract string ToSExpression();
    public abstract string ToBbnf();
  }
}