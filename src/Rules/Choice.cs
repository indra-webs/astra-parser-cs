using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Choice
    : Rule,
      IRule<Choice>,
      Rule.IPart {

    public static new Choice Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is not null);
      Contract.Requires(parent is not null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<Pipe>()) {
        cursor.Skip();
        Rule right = Rule.Parse(cursor, context);
        List<Rule> rules = [];

        if(right is Choice choice) {
          rules.AddRange(choice.Rules);
        }
        else {
          rules.Add(right);
        }

        if(seq![^1] is Choice left) {
          rules.AddRange(left.Rules);
        }
        else {
          rules.Add(seq[^1]);
        }

        return new Choice(
          parent!,
          rules
        );
      }
      else {
        throw new InvalidDataException("Expected a pipe to indicate a choice between rules.");
      }
    }

    public IReadOnlyList<Rule> Rules { get; }

    public Rule Parent { get; }

    private Choice(Rule parent, IReadOnlyList<Rule> rules)
      => (Parent, Rules) = (parent, rules);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__choice__\n{string.Join("\n", Rules.Select(r => r.ToSExpression().Indent()))})";

    public override string ToBbnf()
      => Rules.Count >= 3
        || Rules.Any(r => r is Choice or Sequence or Tagged)
          ? Rules.Select(r => $"| {r.ToBbnf()}".Indent()).Join('\n')
          : Rules.Join(" | ", r => r.ToBbnf());
  }
}