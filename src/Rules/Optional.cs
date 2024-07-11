using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Optional
  : Rule, IRule<Optional> {
    public static new Optional Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      if(seq is null) {
        cursor.Skip(c => c.Type is IWhitespace);
        if(cursor.Current.Is<OpenBracket>()) {
          cursor.Skip();
          Rule rule = Rule.Parse(cursor, grammar);

          cursor.Skip(c => c.Type is IWhitespace);
          if(cursor.Current.Is<CloseBracket>()) {
            cursor.Skip();
            return new Optional(rule);
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
        Rule prev = seq[^1];
        cursor.Skip();

        return prev is Optional opt
          ? new Optional(opt.Rule)
          : new Optional(prev);
      }
      else {
        throw new InvalidDataException("Expected a question mark to indicate an optional preceeding rule; but no preceeding rule was found.");
      }
    }

    public Rule Rule { get; }

    private Optional(Rule rule)
      => Rule = rule;

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