using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {
  public class Not
: Rule, IRule<Not> {
    public static new Not Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is not null);
      cursor.Skip(c => c.Type is IWhitespace);

      if(cursor.Current.Is<Bang>()) {
        if(cursor.Current.Padding.After.IsAny) {
          throw new InvalidDataException("Unexpected padding after the exclamation mark.");
        }

        cursor.Skip();
        Rule rule = Rule.Parse(cursor, grammar);

        return new Not(rule);
      }
      else {
        throw new InvalidDataException("Expected an exclamation mark to indicate a not rule.");
      }
    }

    public Rule Rule { get; }

    private Not(Rule rule)
      => Rule = rule;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__not__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"!{Rule.ToBbnf()}";
  }
}