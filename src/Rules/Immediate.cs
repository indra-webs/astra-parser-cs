using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {
  public class Immediate
    : Rule,
      IRule<Immediate>,
      Rule.IPart {

    public static new Immediate Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<Dot>() && cursor.Current.Padding.HasAny) {
        cursor.Skip();
        Rule rule = Rule.Parse(cursor, context);

        return new Immediate(
          parent ?? throw new InvalidDataException("Expected a parent rule for an immediate rule."),
          rule
        );
      }
      else {
        throw new InvalidDataException("Expected a dot to indicate a concatenation of rules.");
      }
    }

    public Rule Rule { get; }

    public Rule Parent { get; }

    private Immediate(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__immediate__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $". {Rule.ToBbnf()}";
  }
}