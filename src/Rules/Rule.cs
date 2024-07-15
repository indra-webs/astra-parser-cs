using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

namespace Indra.Astra.Rules {

  public abstract partial class Rule
    : IRule<Rule> {

    /// <summary>
    /// If this rule not generate a complete expression, but instead
    ///   should be used as part of a the larger, containing, named rule.
    /// </summary>
    public virtual bool IsPartial
      => true;

    protected Rule() { }

    public abstract Expression Parse(TokenCursor cursor);
    public abstract string ToSExpression();
    public abstract string ToBbnf();

    public static Rule Parse(TokenCursor cursor, Rule.Parser.Context context) {
      Grammar? grammar = context.Grammar;
      Rule? parent = context.Parent;
      IReadOnlyList<Rule>? seq = context.Sequence;
      Contract.Requires(seq is null);

      List<Rule> rules = [];
      context = context with { Sequence = rules };

      cursor.Skip(c => c.Type is IWhitespace);
      while(!cursor.IsAtEnd && !cursor.Current.Is<SemiColon>()) {
        switch(cursor.Current.Type) {
          case Word when cursor.Current.Text.IsUpper():
            rules.Add(Terminal.Parse(cursor, context));
            break;
          case Word when cursor.Current.Text.IsLower():
            rules.Add(Reference.Parse(cursor, context));
            break;
          case Dot:
            rules.Add(Immediate.Parse(cursor, context));
            break;
          case OpenParenthesis:
            rules.Add(Group.Parse(cursor, context));
            break;
          case OpenBracket:
            rules.Add(Optional.Parse(cursor, context));
            break;
          case Question:
            rules[^1] = Optional.Parse(cursor, context);
            break;
          case Star:
            rules[^1] = NoneOrMore.Parse(cursor, context);
            break;
          case Plus:
            rules[^1] = OneOrMore.Parse(cursor, context);
            break;
          case Pipe:
            rules[^1] = Choice.Parse(cursor, context);
            break;
          case Hash:
            rules[^1] = Tagged.Parse(cursor, context);
            break;
          case IWhitespace:
            cursor.Skip();
            continue;
          default:
            throw new InvalidDataException($"Unexpected token: {cursor.Current}");
        }
      }

      // if the first rule is a tagged rule without a target;
      // then we apply the tags to the entire rule/sequence
      if(rules.First() is Tagged ownTags && ownTags.Rule is null) {
        rules.RemoveAt(0);
        if(rules.Count == 1) {
          ownTags.Rule = rules[0];
        }
        else {
          Rule rule = Sequence.Parse(
            cursor,
            context with {Sequence = rules.Skip(1).ToList() }
          );
          ownTags.Rule = rule;
        }

        return ownTags;
      } // return either the single rule or a sequence (without applying own-tags)
      else {
        return rules.Count > 1
          ? Sequence.Parse(cursor, context)
          : rules[0];
      }
    }
  }
}