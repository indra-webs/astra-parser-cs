namespace Indra.Astra {

  public enum FileType {
    Axa,
    Stx,
    Prx,
    Blx,
    Mup,
    Rules
  }

  public partial class Parser(Parser.Config config = null!) {

    #region Private Fields
    private readonly Lazy<Lexer> _lexer
      = new();

    private readonly Lazy<Grammar> _grammar
      = new(() => new Grammar(
        Grammar.Sources.Shared,
        Grammar.Sources.Astra,
        Grammar.Sources.Markup,
        Grammar.Sources.XLLogic,
        Grammar.Sources.Prox,
        Grammar.Sources.Strux,
        Grammar.Sources.Blox
      ));
    #endregion

    /// <summary>
    /// The configuration for the lexer.
    /// </summary>
    public record Config;

    /// <summary>
    /// The grammar used by this parser.
    /// </summary>
    public Grammar Grammar
      => _grammar.Value;

    /// <summary>
    /// The lexer used by this parser.
    /// </summary>
    public Lexer Lexer
      => _lexer.Value;

    /// <summary>
    /// The configuration settings for this parser.
    /// </summary>
    public Config Settings { get; }
      = config ?? new();

    public Result Parse(Input input)
      => input.FileType switch {
        FileType.Axa => ParseAxa(input.Lexed),
        FileType.Stx => ParseStx(input.Lexed),
        FileType.Prx => ParsePrx(input.Lexed),
        FileType.Blx => ParseBlx(input.Lexed),
        FileType.Mup => ParseMup(input.Lexed),
        FileType.Rules => ParseRules(input.Lexed),
        _ => throw new InvalidDataException("Unknown file type.")
      };

    public Result ParseRules(Lexer.Result lexed) {

    }
  }
}