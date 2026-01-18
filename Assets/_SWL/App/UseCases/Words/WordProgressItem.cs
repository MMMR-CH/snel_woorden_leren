namespace SWL.App.UseCases.Words
{
    public readonly struct WordProgressItem
    {
        public readonly string WordId;
        public readonly bool Unlocked;
        public readonly bool Learned;

        public WordProgressItem(string wordId, bool unlocked, bool learned)
        {
            WordId = wordId;
            Unlocked = unlocked;
            Learned = learned;
        }
    }
}
