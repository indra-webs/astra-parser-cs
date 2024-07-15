using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Group
    : Rule,
      IRule<Group>,
      Rule.IPart {

    public static new Group Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<OpenParenthesis>()) {
        cursor.Skip();
        Rule rule = Rule.Parse(cursor, context);

        cursor.Skip(c => c.Type is IWhitespace);
        if(cursor.Current.Is<CloseParenthesis>()) {
          cursor.Skip();
          return new Group(
            parent ?? throw new InvalidDataException("Expected a parent rule for a grouped rule."),
            rule
          );
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

    public Rule Parent { get; }

    private Group(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__group__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => $"({Rule.ToBbnf()})";
  }
}