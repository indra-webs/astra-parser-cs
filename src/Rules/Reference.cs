using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public class Reference
    : Rule,
      IRule<Reference>,
      Rule.IPart {

    public static new Reference Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<Word>() && cursor.Current.Text.IsLower()) {
        string key = cursor.Current.Text;
        cursor.Skip();

        // compound lookup
        if(cursor.Current.Is<Dot>() && cursor.Current.Padding.HasNone && cursor.Next?.Type is Word && cursor.Next.Text.IsLower()) {
          key += cursor.Current.Text + cursor.Next.Text;
          cursor.Skip(2);
        }

        Reference @ref = new(
          parent ?? throw new InvalidDataException("Expected a parent rule for the reference."),
          key
        );
        grammar?._registerReference(@ref, grammar.Context, cursor.Position);

        return @ref;
      }
      else {
        throw new InvalidDataException("Expected a lowercase identifier for a rule reference.");
      }
    }

    /// <summary>
    /// The name of the rule to reference.
    /// </summary>
    public string Key { get; }

    public Rule Parent { get; }

    private Reference(Rule parent, string key)
      => (Parent, Key) = (parent, key);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"({Key.ToSnakeCase().ToLowerInvariant()})";

    public override string ToBbnf()
      => Key.ToSnakeCase().ToLowerInvariant();
  }
}