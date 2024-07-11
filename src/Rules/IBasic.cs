namespace Indra.Astra.Rules {
  public interface IRule<T>
    where T : Rule, IRule<T> {
    public abstract static T Parse(
      TokenCursor atCursor,
      Grammar forGrammar,
      IReadOnlyList<Rule>? currentSeq = null
    );
  }
}