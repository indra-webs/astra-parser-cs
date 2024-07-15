using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {
  public class Not
    : Rule,
      IRule<Not>,
      Rule.IPart {
    public static new Not Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<Bang>()) {
        if(cursor.Current.Padding.After.IsAny) {
          throw new InvalidDataException("Unexpected padding after the exclamation mark.");
        }

        cursor.Skip();
        Rule rule = Rule.Parse(cursor, context);

        return new Not(
          parent ?? throw new InvalidDataException("Expected a parent rule for a not rule."),
          rule: rule
        );
      }
      else {
        throw new InvalidDataException("Expected an exclamation mark to indicate a not rule.");
      }
    }

    public Rule Rule { get; }
    public Rule Parent { get; }

    private Not(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__not__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"!{Rule.ToBbnf()}";
  }
}