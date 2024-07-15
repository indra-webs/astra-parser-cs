namespace Indra.Astra.Rules {
  public partial class Rule {
    public static partial class Parser {

      #region Private Fields

      private static readonly Lazy<Lexer> _lexer
        = new();

      #endregion

      public static Result Parse(string path, Grammar? grammar = null)
        => File.Exists(path)
          ? ParseFromFile(path, grammar)
          : Directory.Exists(path)
            ? ParseFromDirectory(path, grammar)
            : ParseSourceCode(path, grammar);

      public static Result ParseFromFile(string path, Grammar? grammar = null)
        => ParseSourceCode(File.ReadAllText(path), grammar);

      public static Result ParseFromDirectory(string path, Grammar? grammar = null) {
        List<Rule> rules = [];

        try {
          Directory.GetFiles(path, "*.rule", SearchOption.AllDirectories)
            .DebugEach($"Loading all rules from file: {0}.")
            .Select(File.ReadAllText)
            .ForEach(src => _parseRulesFromSourceCode(src, grammar)
              .ForEach(rules.Add));

          return new Success(grammar, [.. rules]);
        }
        catch(Exception e) {
#if DEBUG
          Console.WriteLine($"Failed to parse rules from directory: {path}.");
          Console.WriteLine(e.Message);
#endif

          return new Failure(grammar, e, [.. rules]);
        }
      }

      public static Result ParseSourceCode(string src, Grammar? grammar = null) {
        List<Rule> rules = [];

        try {
          _parseRulesFromSourceCode(src, grammar)
            .ForEach(rules.Add);

          return new Success(grammar, [.. rules]);
        }
        catch(Exception e) {
#if DEBUG
          Console.WriteLine($"Failed to parse rules from source code.");
          Console.WriteLine(e.Message);
#endif

          return new Failure(grammar, e, [.. rules]);
        }
      }

      public static Result ParseFromSource(Grammar.Source source, Grammar? grammar = null) {
        if(Directory.Exists(source.Path)) {
#if DEBUG
          Console.WriteLine($"Loading context: '{source.Key}' from: '{source.Path}'.");
#endif
        }
        else {
          throw new ArgumentException($"Path {source.Path} for Rule Context with key: '{source.Key}' does not exist.");
        }

        Result result
          = ParseFromDirectory(source.Path);

        return result is Success success
          ? new Success(grammar, success.Rules)
          : result;
      }

      public static Custom ParseCustomRule(string source, Grammar? grammar)
        => _lexer.Value.Lex(source) is Lexer.Success success
          ? Custom.Parse(new(success.Tokens), new(grammar))
          : throw new InvalidDataException("Failed to lex the source code.");

      private static IEnumerable<Rule> _parseRulesFromSourceCode(string src, Grammar? grammar = null)
         => src.Split(";\n")
          .Select(s => s.Trim())
          .Where(s => !string.IsNullOrWhiteSpace(s))
          .DebugEach(code => $"Parsing rule: \n{code.Indent()}.")
          .Select(source => ParseCustomRule(source, grammar));
    }
  }
}