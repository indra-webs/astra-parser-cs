using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Text;

using Indra.Astra.Rules;

using ReadOnlyRuleDictionary
  = System.Collections.ObjectModel
    .ReadOnlyDictionary<
      string,
      System.Collections.ObjectModel
        .ReadOnlyDictionary<string, Indra.Astra.Rules.Rule>
    >;

namespace Indra.Astra {

  public partial class Grammar
    : ICloneable {

    #region Private Fields

    private Dictionary<
      string,
      (Rule rule, TextCursor.Location pos, Source ctx)
    >? _refs
      = [];

    #endregion

    /// <summary>
    /// The available variants of the grammer via the 
    ///   different styntax contexts available to it.
    /// </summary>
    public IReadOnlyList<Source> Contexts { get; private init; }

    /// <summary>
    /// The current style of the grammar. 
    /// Determines which syntax to use as the default
    ///   overriding context for rule resolution.
    /// </summary>
    public Source Context { get; set; }

    /// <summary>
    /// The rules given the current context.
    /// </summary>
    public RuleDictionary Rules { get; private init; }

    #region Initialization

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    /// <summary>
    /// Clone constructor.
    /// </summary>
    private Grammar() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Grammar(params Source[] contexts)
      : this() {
      // require at least one valid context
      Contract.Requires(contexts.Length > 0);

      // init context
      Contexts = contexts;
      Context = Contexts[0];

      // load rules
      ReadOnlyRuleDictionary rules = loadRules();
      Rules = new RuleDictionary(this, rules);

      if(!validateRefs(out string? message)) {
        throw new ArgumentException(message);
      }

      // init rules dictionary

      #region Local Helper Methods

      /// <summary> 
      /// load all rules from all contexts, storing them in a Dictionary by key
      /// </summary>
      ReadOnlyRuleDictionary loadRules() {
        Dictionary<string, Dictionary<string, Rule>> ctx
          = [];

        Console.WriteLine("Loading all rules from all contexts:"
          + string.Join("\n  - ", Contexts.Select(c =>
            $"{c.Key}: {c.Path}")));

        // load each context
        foreach(Source context in Contexts) {
          // load each rule from the context
          Dictionary<string, Rule> rules = [];
          if(!ctx.TryAdd(context.Key, rules)) {
            throw new ArgumentException(
              $"Context with key: '{context.Key}' already exists.");
          }

          Context = context;
          var result = Rule.Parser.ParseFromSource(context, this);

        }

        Console.WriteLine("All rules loaded successfully!");

        Context = Contexts[0];
        return ctx.ToDictionary(
          c => c.Key,
          c => c.Value.AsReadOnly()
        ).AsReadOnly();
      }

      /// <summary>
      /// validate all references within the provided grammar rules
      /// </summary>
      bool validateRefs([NotNullWhen(false)] out string? message) {
        Dictionary<string, (Rule rule, TextCursor.Location pos, Source ctx)> refs
          = _refs!;
        _refs = null;

        foreach((string k, (Rule r, TextCursor.Location p, Source c)) in refs) {
          string? ctx, name = null!;
          if(k.Contains('.')) {
            string[] parts = k.Split('.');
            ctx = parts[0];
            name = parts[1];

            if(!rules.ContainsKey(ctx)) {
              message = error(
                $"The context:'{ctx}' does not exist in the current grammar.");

              return false;
            }
            else if(!Rules.Contexts[ctx].ContainsKey(name)) {
              message = error(
                $"The rule:'{name}' does not exist in the context:'{ctx}'.");

              return false;
            }
          }
          else {
            if(!Rules.ContainsKey(k)) {
              message = error(
                $"The rule:'{k}' does not exist in the current grammar.");

              return false;
            }
          }

          #region Local Helper Methods

          string error(string message) {
            Rule named = r;
            while(named is Rule.IPart part) {
              named = part.Parent;
            }

            return $"""
              [ERROR]: Failed to validate all references within the provided grammar rules.
                [Message]: {message.Indent(inline: true)}
                [Location]: [{p.Line}, {p.Column}]
                  [file]: {c.Path}
                  [rule]: {((Named)named).Key}
                  [token]: {k}
                  [line]: {p.Line}
                  [column]: {p.Column}
                  [Index]: {p.Index}
              """;
          }

          #endregion
        }

        message = null;
        return true;
      }

      #endregion
    }
    internal void _registerReference(
      Reference rule,
      Source ctx,
      TextCursor.Location pos
    ) => _refs!.Add(rule.Key, (rule, pos, ctx));

    #endregion

    public string ToSExpression(params string[] contexts) {
      StringBuilder sb = new($"(grammar #ctx:{Context.Key}\n");
      if(contexts.Length == 0) {
        foreach(Rule rule in Rules.Values) {
          sb.Append(rule.ToSExpression().Indent());
          sb.AppendLine();
        }
      }
      else {
        foreach(Source context
        in contexts.Select(c => Contexts.First(c => c.Key == c))) {
          sb.Append($"\t\n(@{context.Key} #ctx");
          foreach(Rule rule in Rules.From(context).Values) {
            sb.Append(rule.ToSExpression().Indent(2));
            sb.AppendLine();
          }
          sb.Append(')');
        }
      }

      return sb.Append(')').ToString();
    }

    public object Clone()
      => new Grammar {
        Contexts = Contexts,
        Context = Context,
        Rules = Rules
      };
  }
}