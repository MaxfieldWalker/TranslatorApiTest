namespace TranslatorApiTest
{
    public interface ILanguageNode
    {
        ILanguageNode NextNode { get; set; }
        string Locale { get; }
        string NextLocale { get; }
        ILanguageNode Connect(ILanguageNode node);
    }

    public interface ITranslationSource
    {
        string Translate(string text);
    }
}
