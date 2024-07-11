using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

using Meep.Tech.Collections;
using Meep.Tech.Text;

namespace Indra.Astra.Rules {
  public class Choice
  : Rule, IBasic<Choice> {
    public static new Choice Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is not null);
      cursor.Skip(c => c.Type is IWhitespace);

      if(cursor.Current.Is<Pipe>()) {
        cursor.Skip();
        Rule right = Rule.Parse(cursor, grammar);
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

        return new Choice(rules);
      }
      else {
        throw new InvalidDataException("Expected a pipe to indicate a choice between rules.");
      }
    }

    public IReadOnlyList<Rule> Rules { get; }

    private Choice(IReadOnlyList<Rule> rules)
      => Rules = rules;

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