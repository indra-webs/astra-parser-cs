using Indra.Astra.Tokens;
using Indra.Astra.Expressions;
using System.Diagnostics.Contracts;

namespace Indra.Astra.Rules {
  public class Optional
    : Rule,
      IRule<Optional>,
      Rule.IPart {
    public static new Optional Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is not null);

      if(seq is null) {
        cursor.Skip(c => c.Type is IWhitespace);
        if(cursor.Current.Is<OpenBracket>()) {
          cursor.Skip();
          Rule rule = Rule.Parse(cursor, context);

          cursor.Skip(c => c.Type is IWhitespace);
          if(cursor.Current.Is<CloseBracket>()) {
            cursor.Skip();
            return new Optional(parent!, rule);
          }
          else {
            throw new InvalidDataException("Expected a right bracket to end an optional rule.");
          }
        }
        else {
          throw new InvalidDataException("Expected a left bracket to start an optional rule.");
        }
      }
      else if(cursor.Current.Is<Question>()) {
        Contract.Requires(seq.Count > 0);
        Rule prev = seq[^1];
        cursor.Skip();

        return prev is Optional opt
          ? new Optional(parent!, opt.Rule)
          : new Optional(parent!, prev);
      }
      else {
        throw new InvalidDataException("Expected a question mark to indicate an optional preceeding rule; but no preceeding rule was found.");
      }
    }

    public Rule Rule { get; }

    public Rule Parent { get; }

    private Optional(Rule parent, Rule rule)
      => (Parent, Rule) = (parent, rule);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__optional__\n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => Rule is Sequence or Choice or Tagged
        ? $"[\n{Rule.ToBbnf().Indent()}]"
        : $"{Rule.ToBbnf()}?";
  }
}