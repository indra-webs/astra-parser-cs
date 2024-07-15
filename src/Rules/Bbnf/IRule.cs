namespace Indra.Astra.Rules {

  /// <summary>
  /// A BBNF Parseable Rule.
  /// </summary>
  public interface IRule;

  /// <inheritdoc cref="IRule"/>
  public interface IRule<TSelf>
    : IRule
    where TSelf : Rule, IRule<TSelf> {

    /// <summary>
    /// Used to parse this type of rule from a Tokenized BBNF source text.
    /// </summary>
    public abstract static TSelf Parse(
      TokenCursor atCursor,
      Rule.Parser.Context context
    );
  }
}