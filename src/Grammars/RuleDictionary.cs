using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Indra.Astra.Rules;
using Indra.Astra.Tokens;

using Meep.Tech.Collections;

using ReadOnlyRuleDictionary
  = System.Collections.ObjectModel
    .ReadOnlyDictionary<
      string,
      System.Collections.ObjectModel
        .ReadOnlyDictionary<string, Indra.Astra.Rules.Rule>
    >;

namespace Indra.Astra {

  public partial class Grammar {

    public class RuleDictionary
      : IReadOnlyDictionary<string, Rule> {

      #region Private Fields
      private ReadOnlyDictionary<string, Rule> _current
        => Contexts[Grammar.Context.Key];
      #endregion

      #region Properties

      public Grammar Grammar { get; }

      public ReadOnlyRuleDictionary Contexts { get; }

      public int Count
        => Contexts.Values.Sum(c => c.Count);

      public IEnumerable<string> Keys
        => Contexts.Values.SelectMany(c => c.Keys).Distinct();

      public IEnumerable<Rule> Values
        => Keys.Select(k => this[k]);

      #endregion

      #region Indexers

      public ReadOnlyDictionary<string, Rule> this[Source context]
        => Contexts[context.Key];


      public Rule this[string key]
        => TryGetValue(key, out Rule? value)
          ? value
          : throw new KeyNotFoundException($"Rule with key: '{key}' not found in any known context.");

      #endregion

      #region Initialization

      internal RuleDictionary(
        Grammar source,
        ReadOnlyRuleDictionary rules
      ) => (Grammar, Contexts)
        = (source, rules);

      #endregion

      public ReadOnlyDictionary<string, Rule> From(Source context)
        => Contexts[context.Key];

      public ReadOnlyDictionary<string, Rule> From(string context)
        => Contexts[context];

      public bool ContainsKey(string key)
        => Contexts.Values.Any(c => c.ContainsKey(key));

      public bool TryGetValue(string key, [MaybeNullWhen(false)] out Rule value) {
        if(_current.TryGetValue(key, out value)) {
          return true;
        }
        else {
          foreach(Source context in Grammar.Contexts.Where(not: c => c.Key == Grammar.Context.Key)) {
            if(Contexts[context.Key].TryGetValue(key, out value)) {
              return true;
            }
          }

          return false;
        }
      }

      public IEnumerator<KeyValuePair<string, Rule>> GetEnumerator()
        => Contexts
          .OrderBy(c => Grammar.Context.Key != c.Key
            ? Grammar.Contexts.IndexOf(c.Key)
            : -1
          ).SelectMany(c => c.Value)
          .GroupBy(r => r.Key)
          .Select(g => g.First())
          .GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    }
  }
}