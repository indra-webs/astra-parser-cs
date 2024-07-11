using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;
using Indra.Astra.Expressions;

using Meep.Tech.Collections;
using Meep.Tech.Text;

namespace Indra.Astra.Rules {
  public abstract class Custom
    : Rule, IRule<Custom> {

    public static new Custom Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      string name = null!;
      try {
        Contract.Requires(seq is null);
        cursor.Skip(c => c.Type is IWhitespace);

        if(cursor.Current.Is<Word>()) {
          name = cursor.Current.Text;

          cursor.Skip();
          cursor.Skip(c => c.Type is IWhitespace);
          if(cursor.Current.Is<DoubleColon>()
            && (cursor.Next?.Is<Equal>() ?? false)
            && cursor.Current.Padding.After.IsNone
          ) {
            cursor.Skip(2);

            Rule rule = Rule.Parse(cursor, grammar);
            return name.StartsWith('_')
              ? new Hidden(name, rule)
              : new Named(name, rule);
          }
          else {
            throw new InvalidDataException("Expected `::=` as an assigner following the rule's key.");
          }
        }
        else {
          throw new InvalidDataException("Expected an identifier for a rule key.");
        }
      }
      catch(Exception e) {
        throw new InvalidDataException($"""
        [ERROR]: Failed to parse custom rule with key: {name ?? "???"}.
          [Message]: {e.Message.Indent(inline: true)}
          [Location]: [{cursor.Current.Line}, {cursor.Current.Column}]
            [file]: {grammar.Context.Path}
            [token]: {cursor.Current.Name.ToSnakeCase().ToUpperInvariant()}{"{"}{cursor.Current.Text}{"}"}
            [line]: {cursor.Current.Line}
            [column]: {cursor.Current.Column} 
            [Index]: {cursor.Current.Index}
        """, e);
      }
    }

    public Rule Rule { get; }

    public string Name { get; }

    protected Custom(string name, Rule rule) {
      Name = name;
      Rule = rule;
    }

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"({Name} ::= \n{Rule.ToSExpression().Indent()})";

    public override string ToBbnf()
      => Rule is Tagged tagged
      ? $"{Name} ::= {tagged.TagsToBbnf()}\n{tagged.Rule.ToBbnf().Indent()};"
      : $"{Name} ::=\n{Rule.ToBbnf().Indent()};";
  }
}