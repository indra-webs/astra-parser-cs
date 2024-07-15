using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class OneOrMore
    : Rule,
      IRule<OneOrMore>,
      Rule.IPart {

    public static new OneOrMore Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<Plus>() && cursor.Current.Padding.Before.IsNone) {
        cursor.Skip();
        return new OneOrMore(
          parent ?? throw new InvalidDataException("Expected a parent rule for a one or more rule."),
          seq![^1]
        );
      }
      else {
        throw new InvalidDataException("Expected a plus to indicate one or more of a rule.");
      }
    }

    public Rule Rule { get; }

    public Rule Parent { get; }

    private OneOrMore(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__oom__\n\t{Rule.ToSExpression()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}+";
  }
}