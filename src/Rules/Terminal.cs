using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {
  public class Terminal
    : Rule,
      IRule<Terminal>,
      Rule.IPart {

    public static new Terminal Parse(TokenCursor cursor, Parser.Context context) {
      Rule? parent = context.Parent;
      Contract.Requires(parent is not null);

      if(cursor.Current.Is<Word>() && cursor.Current.Text.IsUpper()) {
        string key = cursor.Current.Text;
        TokenType type
          = Types.Get(key,
            System.Globalization.CompareOptions.IgnoreSymbols
              | System.Globalization.CompareOptions.IgnoreCase);

        cursor.Skip();
        return new Terminal(parent!, type);
      }
      else {
        throw new InvalidDataException(
          "Expected an upper-case word token for a terminal token rule.");
      }
    }

    public TokenType Type { get; }

    public Rule Parent { get; }

    private Terminal(Rule parent, TokenType token)
      => (Parent, Type) = (parent, token);

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"({ToBbnf()})";

    public override string ToBbnf()
      => Type.Name.ToSnakeCase().ToUpperInvariant();
  }
}