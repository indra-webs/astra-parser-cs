namespace Indra.Astra
{

  public partial class Parser
  {
    /// <summary>
    /// The input to the parser.
    /// </summary>
    public record Input(
  Lexer.Result Lexed,
  FileType FileType = FileType.Axa
)
    {
      public static implicit operator Input(Lexer.Result lexed)
        => new(lexed);

      public static implicit operator Input(string input)
        => new(input);
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