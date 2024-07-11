namespace Indra.Astra.Rules {
  public interface IBasic<T>
    where T : Rule, IBasic<T> {
    public abstract static T Parse(
      TokenCursor atCursor,
      Grammar forGrammar,
      IReadOnlyList<Rule>? currentSeq = null
    );
  }
}