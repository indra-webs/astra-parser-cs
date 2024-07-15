namespace Indra.Astra.Rules {
  public partial class Rule {
    public static partial class Parser {

      public readonly record struct Context {
        public Grammar? Grammar { get; }
        public Rule? Parent { get; internal init; }
        public IReadOnlyList<Rule>? Sequence { get; internal init; }
        internal Context(
          Grammar? grammar,
          Rule? parent = null,
          IReadOnlyList<Rule>? currentSeq = null
        ) => (Grammar, Parent, Sequence)
          = (grammar, parent, currentSeq);
      }
    }
  }
}