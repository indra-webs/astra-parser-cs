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

  public partial class Grammar {

    #region Private Fields
    private readonly Dictionary<string, (int, int)>? _refs = [];
    #endregion

    /// <summary>
    /// The available variants of the grammer via the different styntax contexts available to it.
    /// </summary>
    public IReadOnlyList<Source> Contexts { get; }

    /// <summary>
    /// The current style of the grammar. Determines which syntax to use as the default overriding context for rule resolution.
    /// </summary>
    public Source Context { get; set; }

    /// <summary>
    /// The rules given the current context.
    /// </summary>
    public RuleDictionary Rules { get; }

    #region Initialization

    public Grammar(params Source[] contexts) {
      // require at least one valid context
      Contract.Requires(contexts.Length > 0);

      // init context
      Contexts = contexts;
      Context = Contexts[0];

      // load rules
      ReadOnlyRuleDictionary rules = loadRules();
      if(!validateRules(rules, out string? message)) {
        throw new ArgumentException(message);
      }

      // init rules dictionary
      Rules = new RuleDictionary(this, rules);

      #region Local Helper Methods

      /// <summary> 
      /// load all rules from all contexts, storing them in a dictionary by context key
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
          if(ctx.TryAdd(context.Key, rules)) {
            if(Directory.Exists(context.Path)) {
              Console.WriteLine($"Loading context: '{context.Key}' from: '{context.Path}'.");
            }
            else {
              throw new ArgumentException($"Path {context.Path} for Rule Context with key: '{context.Key}' does not exist.");
            }
          }
          else {
            throw new ArgumentException($"Context with key: '{context.Key}' already exists.");
          }

          Context = context;
          Directory.GetFiles(context.Path, "*.rule", SearchOption.AllDirectories)
            .ForEach(f => Console.WriteLine($"Loading all rules from file: {f} for context: {context.Key}."))
            .Select(File.ReadAllText)
            .SelectMany(text =>
              text.Split(";\n")
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
            ).ForEach(code => Console.WriteLine($"Parsing rule: \n{code.Indent()}."))
            .Select(source => Rule.Parse(source, this))
            .ForEach(rule => rules.Add(
              rule.Name,
              rule))
            .ForEach(rule => Console.WriteLine($"Sucessfully added rule: {rule.Name}, to context: {context.Key}."));
        }

        Console.WriteLine("All rules loaded successfully!");

        Context = Contexts[0];
        return ctx.ToDictionary(
          c => c.Key,
          c => c.Value.AsReadOnly()
        ).AsReadOnly();
      }

      bool validateRules(ReadOnlyRuleDictionary readOnlyDictionary, [NotNullWhen(false)] out string? message) {
        _refs.ForEach(r => {
          if(!readOnlyDictionary.ContainsKey(r)) {
            message = $"Reference: '{r}' not found in any context.";
            return false;
          }
        });

        _refs = null;
      }

      #endregion
    }

    internal void _registerReference(string name)
      => _refs!.Add(name);

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
        foreach(Source context in contexts.Select(c => Contexts.First(c => c.Key == c))) {
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
  }
}