using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

using Meep.Tech.Collections;

namespace Indra.Astra.Rules {
  public class OneOrMore
  : Rule, IRule<OneOrMore> {
    public static new OneOrMore Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is not null);
      cursor.Skip(c => c.Type is IWhitespace);

      if(cursor.Current.Is<Plus>() && cursor.Current.Padding.Before.IsNone) {
        cursor.Skip();
        return new OneOrMore(seq![^1]);
      }
      else {
        throw new InvalidDataException("Expected a plus to indicate one or more of a rule.");
      }
    }

    public Rule Rule { get; }

    private OneOrMore(Rule rule)
      => Rule = rule;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__oom__\n\t{Rule.ToSExpression()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()}+";
  }
}