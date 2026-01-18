namespace SWL.Core.Domain.Levels
{
    public enum LevelType
    {
        Match_Words_Images = 0,
        Match_Words_Sentences = 1,
        Choose_Words_Image_Text = 2,
        Unique = 99,

        // Sub-level types
        Fill_In_The_Blank = 100,
        Crossword_Puzzle = 101
    }
}
