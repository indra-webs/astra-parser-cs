using Indra.Astra.OldRules;

namespace Indra.Astra {

  public enum FileType {
    Axa,
    Stx,
    Prx,
    Blx,
    Mup
  }

  public partial class Parser {
    #region Private Fields
    private readonly Lazy<OldRules.Types> _rules;
    #endregion

    /// <summary>
    /// The configuration for the lexer.
    /// </summary>
    public record Config;

    /// <summary>
    /// The input to the parser.
    /// </summary>
    public record Input(
      Lexer.Result Lexed,
      FileType FileType = FileType.Axa
    ) {
      #region Private Fields
      internal static readonly Dictionary<Thread, Lexer.Result> _threads
        = [];
      internal static Input _current
        => _threads[Thread.CurrentThread];
      #endregion

      public static implicit operator Input(Lexer.Result lexed)
        => new(lexed);

      public static implicit operator Input(string input)
        => new(input);
    }

    /// <summary>
    /// The configuration settings for this parser.
    /// </summary>
    public Config Settings { get; }

    /// <summary>
    /// The compiled rules for this parser.
    /// </summary>
    public OldRules.Types Rules
      => _rules.Value;

    public Parser(Config config = null!) {
      Settings = config ?? new();
      _rules = new(() => new(Settings));
    }

    public void Parse(Lexer.Result input) {
      Input._threads[Thread.CurrentThread] = input;

    }
  }
}

// TODO: toss
// [CollectionBuilder(typeof(Group), nameof(Build))]
// public class Group(params Expression[] expressions)
//     : Expression, IEnumerable<Expression> {

//   public static Group Of(params Expression[] expressions)
//     => new(expressions);

//   public static Group From(IEnumerable<Expression> expressions)
//     => new(expressions.ToArray());

//   public static Group Build(ReadOnlySpan<Expression> expressions)
//     => new(expressions.ToArray());

//   public Expression[] Expressions { get; } = expressions;

//   public static implicit operator Group((Expression left, Expression right) options)
//     => [options.left, options.right];

//   public static implicit operator Group(Expression[] expressions)
//     => new(expressions);

//   public IEnumerator<Expression> GetEnumerator()
//     => ((IEnumerable<Expression>)Expressions).GetEnumerator();

//   IEnumerator IEnumerable.GetEnumerator()
//     => GetEnumerator();
// }