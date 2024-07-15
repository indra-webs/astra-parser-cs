using System.Diagnostics.CodeAnalysis;

using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Sequence
    : Rule,
      IRule<Sequence>,
      Rule.IPart {
    public static new Sequence Parse(TokenCursor cursor, Rule.Parser.Context context)
      => context.Sequence is null || context.Sequence.Count < 2
        ? throw new ArgumentException("Current parser sequence must contain at least two rules.")
        : new Sequence(context.Parent ?? throw new InvalidDataException("Expected a parent rule for a sequence rule."), context.Sequence);

    public IReadOnlyList<Rule> Rules { get; }

    public Rule Parent { get; }

    private Sequence(Rule parent, IReadOnlyList<Rule> rules)
      => (Parent, Rules) = (parent, rules);

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