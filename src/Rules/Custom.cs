using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {
  public abstract class Custom
    : Rule,
      IRule<Custom> {

    public static new Custom Parse(
      TokenCursor cursor,
      Rule.Parser.Context context
    ) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(parent is null);

      string key = null!;
      try {
        Contract.Requires(seq is null);
        cursor.Skip(c => c.Type is IWhitespace);

        if(cursor.Current.Is<Word>()) {
          key = cursor.Current.Text;

          cursor.Skip();
          cursor.Skip(c => c.Type is IWhitespace);
          if(cursor.Current.Is<DoubleColon>()
            && (cursor.Next?.Is<Equal>() ?? false)
            && cursor.Current.Padding.After.IsNone
          ) {
            cursor.Skip(2);

            Rule rule = Rule.Parse(cursor, context);
            return key.StartsWith('_')
              ? new Hidden(key, rule)
              : new Named(key, rule);
          }
          else {
            throw new InvalidDataException(
              "Expected `::=` as an assigner following the rule's key.");
          }
        }
        else {
          throw new InvalidDataException(
            "Expected an alphanumeric identifier/word as the rule key.");
        }
      }
      catch(Exception e) {
        throw new InvalidDataException($"""
        [ERROR]: Failed to parse custom rule with key: {key ?? "???"}.
          [Message]: {e.Message.Indent(inline: true)}
          [Location]: [{cursor.Current.Line}, {cursor.Current.Column}]
            [file]: {grammar?.Context.Path ?? "???"}
            [token]: {cursor.Current.Name
              .ToSnakeCase()
              .ToUpperInvariant()}{"{"}{cursor.Current.Text}{"}"}
            [line]: {cursor.Current.Line}
            [column]: {cursor.Current.Column} 
            [Index]: {cursor.Current.Index}
        """, e);
      }
    }

    public Rule Rule { get; }

    public string Key { get; }

    protected Custom(string key, Rule rule) {
      Key = key;
      Rule = rule;
    }

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"({Key} ::= \n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => Rule is Tagged tagged
      ? $"{Key} ::= {tagged.TagsToBbnf()}\n{tagged.Rule.ToBbnf().Indent()};"
      : $"{Key} ::=\n{Rule.ToBbnf().Indent()};";
  }
}