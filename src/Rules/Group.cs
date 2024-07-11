using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Group
  : Rule, IRule<Group> {
    public static new Group Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<OpenParenthesis>()) {
        cursor.Skip();
        Rule rule = Rule.Parse(cursor, grammar);

        cursor.Skip(c => c.Type is IWhitespace);
        if(cursor.Current.Is<CloseParenthesis>()) {
          cursor.Skip();
          return new Group(rule);
        }
        else {
          throw new InvalidDataException("Expected a right parenthesis to end a grouped rule.");
        }
      }
      else {
        throw new InvalidDataException("Expected a left parenthesis to start a grouped rule.");
      }
    }

    public Rule Rule { get; }

    private Group(Rule rule)
      => Rule = rule;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__group__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"({Rule.ToBbnf()})";
  }
}