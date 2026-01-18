using System;

namespace SWL.Core.Domain.Words
{
    [Serializable]
    public sealed class WordEntry
    {
        public string Id;
        public string Dutch;
        public string Translation;
        public WordArticle Article;
        public string Example; // optional
        public string Category; // optional
    }
}
