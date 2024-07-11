using System.Reflection;

namespace Indra.Astra {
  public partial class Parser {
    public class Flag {
      internal Flag() { }

      public string Name
        => GetType().Name;
    }

    public class Flag<TType> : Flag {
      public static TType Type { get; }
        = (TType)typeof(TType).Assembly.CreateInstance(
          typeof(TType).FullName!,
          false,
          BindingFlags.CreateInstance
            | BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic,
          null, null, null, null
        )!;

      private Flag() { }
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