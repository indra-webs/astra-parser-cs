using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {
  public class NoneOrMore
    : Rule,
      IRule<NoneOrMore>,
      Rule.IPart {

    public static new NoneOrMore Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);

      if(cursor.Current.Is<Star>() && cursor.Current.Padding.Before.IsNone) {
        cursor.Skip();
        return new NoneOrMore(
          parent ?? throw new InvalidDataException("Expected a parent rule for a zero or more rule."),
          seq![^1]
        );
      }
      else {
        throw new InvalidDataException("Expected a star to indicate zero or more repetitions of a rule.");
      }
    }

    public Rule Rule { get; }

    public Rule Parent { get; }

    private NoneOrMore(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__nom__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}*";
  }
}