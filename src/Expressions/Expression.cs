using System.Runtime.CompilerServices;

namespace Indra.Astra.Expressions {
  [CollectionBuilder(typeof(Sequence), nameof(Sequence.Build))]
  public abstract partial class Expression {
    #region Private Fields
    private readonly Lazy<HashSet<Parser.Flag>> _flags
        = new();
    private readonly Lazy<ReadOnlySet<Parser.Flag>> _ro_flags;
    #endregion

    public Rule? Rule { get; private set; }

    public string? Key { get; private set; }

    public ReadOnlySet<Parser.Flag> Flags
      => _ro_flags.Value;

    public Expression()
      => _ro_flags = new(() => new(_flags.Value));
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