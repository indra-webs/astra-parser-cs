using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

using Meep.Tech.Text;

namespace Indra.Astra.Rules {
  public class Terminal
    : Rule, IBasic<Terminal> {
    public static new Terminal Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is null);
      if(cursor.Current.Is<Word>() && cursor.Current.Text.IsUpper()) {

        string key = cursor.Current.Text;
        TokenType type
          = Tokens.Types.Get(key,
            System.Globalization.CompareOptions.IgnoreSymbols
              | System.Globalization.CompareOptions.IgnoreCase);

        cursor.Skip();

        return new Terminal(type);
      }
      else {
        throw new InvalidDataException("Expected an upper-case word token for a terminal token rule.");
      }
    }

    public TokenType Type { get; }

    private Terminal(TokenType token)
      => Type = token;

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"({ToBbnf()})";

    public override string ToBbnf()
      => Type.Name.ToSnakeCase().ToUpperInvariant();
  }
}