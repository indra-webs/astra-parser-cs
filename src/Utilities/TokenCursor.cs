
using System.Diagnostics.CodeAnalysis;
using System.Text;

using Indra.Astra.Tokens;

namespace Indra.Astra {
  public class TokenCursor(IEnumerable<Token> source)
    : Cursor<Token>(source), ITextCursor<Token> {

    #region Private Fields

    private string? _full;

    #endregion

    /// <inheritdoc />
    public new TextCursor.Location Position { get; protected set; }

    /// <inheritdoc />
    public int Line { get; protected set; }

    /// <inheritdoc />
    public int Column { get; protected set; }

    /// <inheritdoc />
    public bool IsStartOfLine { get; protected set; }

    /// <inheritdoc />
    public string Text {
      get {
        if(_full is null) {
          while(!HasReachedEnd) {
            Peek(Buffer + 1);
          }

          _full = Memory[0].GetTextUntil(Memory[^1]);
        }

        return _full;
      }
    }

    public bool Read([NotNull] string match)
      => throw new NotImplementedException();

    public bool ReadNext([NotNull] string match)
      => throw new NotImplementedException();

    #region Explicit Interface Implementations

    TextCursor.ILocation IReadOnlyTextCursor<Token>.Position
      => Position;

    #endregion
  }
}