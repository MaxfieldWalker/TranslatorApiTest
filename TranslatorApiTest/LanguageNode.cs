using System;
using System.Collections;
using System.Collections.Generic;
using TranslatorApiTest.ServiceReference1;
using System.Linq;

namespace TranslatorApiTest
{
    public class LanguageNode : ILanguageNode
    {
        public ILanguageNode NextNode { get; set; }
        public string Locale { get; }
        public string NextLocale => this.NextNode?.Locale;

        public LanguageNode(string locale)
        {
            this.Locale = locale;
        }

        public ILanguageNode Connect(ILanguageNode node)
        {
            this.NextNode = node;
            return node;
        }
    }

    public class LanguageRootNode : LanguageNode, ITranslationSource, IEnumerable<ILanguageNode>
    {
        private LanguageServiceClient translatorService;
        private string token;

        public LanguageRootNode(string locale, LanguageServiceClient translatorService, string token) : base(locale)
        {
            this.translatorService = translatorService;
            this.token = token;
        }

        public string Translate(string text)
        {
            var s = text;
            this.Aggregate((a, b) =>
            {
                s = this.InnerTranslate(s, a.Locale, b.Locale);
                return b;
            });

            return s;
        }

        private string InnerTranslate(string text, string from, string to)
        {
            Console.WriteLine($"Translating: {from} -> {to}");

            // general: 通常翻訳
            // generalnn: ニューラルネットワークを利用した翻訳
            return this.translatorService.Translate(this.token, text, from, to, "text/plain", "generalnn", string.Empty);
        }

        IEnumerator<ILanguageNode> IEnumerable<ILanguageNode>.GetEnumerator()
        {
            ILanguageNode node = this;
            while (node != null)
            {
                yield return node;
                node = node.NextNode;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class LanguageEndNode : LanguageNode
    {
        public LanguageEndNode(string locale) : base(locale)
        {
        }
    }
}
