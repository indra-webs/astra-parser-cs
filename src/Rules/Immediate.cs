using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

using Meep.Tech.Collections;
using Meep.Tech.Text;

namespace Indra.Astra.Rules {
  public class Immediate
    : Rule, IBasic<Immediate> {
    public static new Immediate Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is null);
      cursor.Skip(c => c.Type is IWhitespace);

      if(cursor.Current.Is<Dot>() && cursor.Current.Padding.HasAny) {
        cursor.Skip();
        Rule rule = Rule.Parse(cursor, grammar);

        return new Immediate(rule);
      }
      else {
        throw new InvalidDataException("Expected a dot to indicate a concatenation of rules.");
      }
    }

    public Rule Rule { get; }

    private Immediate(Rule rule)
      => Rule = rule;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__immediate__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $". {Rule.ToBbnf()}";
  }
}