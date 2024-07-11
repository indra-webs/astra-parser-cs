using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

using Meep.Tech.Text;
using Meep.Tech.Collections;

namespace Indra.Astra.Rules {
  public class Reference
    : Rule, IRule<Reference> {
    public static new Reference Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is null);

      cursor.Skip(c => c.Type is IWhitespace);
      if(cursor.Current.Is<Word>() && cursor.Current.Text.IsLower()) {
        string name = cursor.Current.Text;
        cursor.Skip();

        // compound lookup
        if(cursor.Current.Is<Dot>() && cursor.Current.Padding.HasNone && cursor.Next?.Type is Word && cursor.Next.Text.IsLower()) {
          name += cursor.Current.Text + cursor.Next.Text;
          cursor.Skip(2);
        }

        grammar._registerReference(name);
        return new Reference(name);
      }
      else {
        throw new InvalidDataException("Expected a lowercase identifier for a rule reference.");
      }
    }

    /// <summary>
    /// The name of the rule to reference.
    /// </summary>
    public string Key { get; }

    private Reference(string key)
      => Key = key;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"({Key.ToSnakeCase().ToLowerInvariant()})";

    public override string ToBbnf()
      => Key.ToSnakeCase().ToLowerInvariant();
  }
}