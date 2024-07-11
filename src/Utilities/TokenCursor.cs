
using Indra.Astra.Tokens;

using Meep.Tech.Collections;

namespace Indra.Astra {
  public class TokenCursor(IEnumerable<Token> source)
    : Cursor<Token>(source) {

  }
}