using System.Diagnostics.CodeAnalysis;

using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Sequence
  : Rule, IRule<Sequence> {
    public static new Sequence Parse(TokenCursor cursor, Grammar grammar, [NotNull] IReadOnlyList<Rule>? seq = null)
      => seq is null || seq.Count < 2
        ? throw new ArgumentException("Current parser sequence must contain at least two rules.")
        : new Sequence(seq);

    public IReadOnlyList<Rule> Rules { get; }

    private Sequence(IReadOnlyList<Rule> rules)
      => Rules = rules;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__sequence__\n{Rules.Join('\n', r => r.ToSExpression().Indent())})";

    public override string ToBbnf()
      => Rules.Count >= 3
        || Rules.Any(r => r is Choice or Sequence or Tagged)
          ? Rules.Join('\n', r => r.ToBbnf().Indent())
          : Rules.Join(" ", r => r.ToBbnf());
  }
}