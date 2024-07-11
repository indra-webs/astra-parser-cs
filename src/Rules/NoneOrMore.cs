using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {
  public class NoneOrMore
  : Rule, IRule<NoneOrMore> {

    public static new NoneOrMore Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is not null);
      if(cursor.Current.Is<Star>() && cursor.Current.Padding.Before.IsNone) {
        cursor.Skip();
        return new NoneOrMore(seq![^1]);
      }
      else {
        throw new InvalidDataException("Expected a star to indicate zero or more repetitions of a rule.");
      }
    }

    public Rule Rule { get; }

    private NoneOrMore(Rule rule)
      => Rule = rule;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__nom__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}*";
  }
}