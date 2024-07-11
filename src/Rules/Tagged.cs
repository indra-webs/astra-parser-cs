using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

using Indra.Astra.Expressions;
using Indra.Astra.Tokens;

using Meep.Tech.Collections;
using Meep.Tech.Text;

namespace Indra.Astra.Rules {
  public class Tagged
    : Rule, IBasic<Tagged> {
    public static new Tagged Parse(TokenCursor cursor, Grammar grammar, IReadOnlyList<Rule>? seq = null) {
      Contract.Requires(seq is not null);
      cursor.Skip(c => c.Type is IWhitespace);

      Token? start = cursor.Current;
      Dictionary<string, string?>? tags = null;

      if(cursor.Current.Is<Hash>() && cursor.Current.Padding.After.IsNone) {
        while(!cursor.IsAtEnd) {
          // padding before the current token marks the end of a tag
          if(cursor.Current.Padding.Before.IsAny) {
            if(start is not null) {
              endTag();
            }
          }

          // newlines mark the end of a tag list
          if(cursor.Current.Is<NewLine>()) {
            break;
          } // hash marks the start of a new tag
          else if(cursor.Current.Is<Hash>()) {
            startTag();
          }

          cursor.Skip();
        }

        if(start is not null) {
          endTag();
        }

        if(tags is null) {
          throw new InvalidDataException("Tags must be present.");
        }

        if(seq!.Count == 0) {
          return new Tagged(tags, null!);
        }
        else if(seq![^1] is Tagged tagged) {
          tags = tags.Concat(tagged.Tags).ToDictionary();
          return new Tagged(tags, tagged.Rule);
        }
        else {
          return new Tagged(tags, seq[^1]);
        }
      }
      else {
        throw new InvalidDataException("Expected a hash symbol and a space to start a tag after an existing rule.");
      }

      #region Local Helper Methods

      void startTag() {
        if(start is not null) {
          throw new InvalidDataException("tags must be separated by whitespace.");
        }
        else {
          start = cursor.Current;
        }
      }

      void endTag() {
        (string? key, IEnumerable<string>? values)
          = ((Lexer.Success)cursor.Current.Source).GetText(start, cursor.Previous!).Split(":");

        (tags ??= []).Add(
            key ?? throw new InvalidOperationException("Tags must have a key"),
            string.Join(':', values)
        );

        start = null;
      }

      #endregion
    }

    public ReadOnlyDictionary<string, string?> Tags { get; }

    public Rule Rule { get; internal set; }

    private Tagged(Dictionary<string, string?> tags, Rule rule) {
      Tags = new ReadOnlyDictionary<string, string?>(tags);
      Rule = rule;
    }

    public override Expression Parse(TokenCursor cursor)
      => throw new NotImplementedException();

    public override string ToSExpression()
      => $"(__tagged__\n{Rule.ToSExpression().Indent()}\n{TagsToBbnf().Indent()})";

    public override string ToBbnf()
      => $"{Rule.ToBbnf()} #{TagsToBbnf()}";

    public string TagsToBbnf()
      => Tags.Join(' ', TagToBbnf);
    #region Static Helper Methods

    public static string TagToBbnf(string key, string? value)
      => $"#{key}{(value is not null ? $":{value}" : "")}";

    public static string TagToBbnf(KeyValuePair<string, string?> tag)
      => TagToBbnf(tag.Key, tag.Value);

    public static KeyValuePair<string, string?> TagFromBbnf(string tag) {
      string[] parts = tag.Split(':');
      return new KeyValuePair<string, string?>(parts[0], parts.Length > 1 ? parts[1..].Join(":") : null);
    }

    #endregion
  }
}