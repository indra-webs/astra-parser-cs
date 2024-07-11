using System.Diagnostics.Contracts;

using Indra.Astra.Tokens;

using Meep.Tech.Text;
using Meep.Tech.Collections;
using Indra.Astra.Expressions;

namespace Indra.Astra.Rules {

  public abstract partial class Rule
    : IRule<Rule> {

    #region Private Fields

    private static readonly Lazy<Lexer> _lexer
        = new();

    #endregion

    /// <summary>
    /// If this rule not generate a complete expression, but instead
    ///   should be used as part of a the larger, containing, named rule.
    /// </summary>
    public virtual bool IsPartial
      => true;

    public abstract Expression Parse(TokenCursor cursor);

    public static Custom Parse(string source, Grammar grammar)
      => _lexer.Value.Lex(source) is Lexer.Success success
          ? Custom.Parse(new(success.Tokens), grammar)
          : throw new InvalidDataException("Failed to lex the source code.");

    public static Rule Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is null);
      cursor.Skip(c => c.Type is IWhitespace);
      List<Rule> rules = [];

      while(!cursor.IsAtEnd && !cursor.Current.Is<SemiColon>()) {
        switch(cursor.Current.Type) {
          case Word when cursor.Current.Text.IsUpper():
            rules.Add(Terminal.Parse(cursor, grammar));
            break;
          case Word when cursor.Current.Text.IsLower():
            rules.Add(Reference.Parse(cursor, grammar));
            break;
          case Dot:
            rules.Add(Immediate.Parse(cursor, grammar));
            break;
          case OpenParenthesis:
            rules.Add(Group.Parse(cursor, grammar));
            break;
          case OpenBracket:
            rules.Add(Optional.Parse(cursor, grammar));
            break;
          case Question:
            rules[^1] = Optional.Parse(cursor, grammar, rules);
            break;
          case Star:
            rules[^1] = NoneOrMore.Parse(cursor, grammar, rules);
            break;
          case Plus:
            rules[^1] = OneOrMore.Parse(cursor, grammar, rules);
            break;
          case Pipe:
            rules[^1] = Choice.Parse(cursor, grammar, rules);
            break;
          case Hash:
            rules[^1] = Tagged.Parse(cursor, grammar, rules);
            break;
          case IWhitespace:
            cursor.Skip();
            continue;
          default:
            throw new InvalidDataException($"Unexpected token: {cursor.Current}");
        }
      }

      // if the first rule is a tagged rule without a target; then we apply the tags to the entire rule/sequence
      if(rules.First() is Tagged ownTags && ownTags.Rule is null) {
        rules.RemoveAt(0);
        if(rules.Count == 1) {
          ownTags.Rule = rules[0];
        }
        else {
          Rule rule = Sequence.Parse(cursor, grammar, rules.Skip(1).ToList());
          ownTags.Rule = rule;
        }

        return ownTags;
      } // return either the single rule or a sequence (without applying own-tags)
      else {
        return rules.Count > 1
          ? Sequence.Parse(cursor, grammar, rules)
          : rules[0];
      }
    }

    protected Rule() { }

    public abstract string ToSExpression();
    public abstract string ToBbnf();
  }
}