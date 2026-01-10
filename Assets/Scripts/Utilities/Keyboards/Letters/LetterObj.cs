using System;
using System.Threading;
using UnityEngine;

namespace MC.Modules.Keyboard
{
    [Serializable]
    public enum Letter
    {
        Null = default, None = -1, NewLine = 10, Space = 32, Underline = 95,
        QuestionMark = 63, ExclamationMark = 33, Comma = 44, Period = 46, Colon = 58, Semicolon = 59,
        SingleQuote = 39, DoubleQuote = 34,
        A = 65, B = 66, C = 67, D = 68, E = 69, É = 201, È = 200, Ë = 203, F = 70, G = 71, H = 72, I = 73, J = 74, K = 75, L = 76, M = 77,
        N = 78, O = 79, Ó = 211, P = 80, Q = 81, R = 82, S = 83, T = 84, U = 85, V = 86, W = 87, X = 88, Y = 89, Z = 90,
        Ç = 199, Ğ = 286, İ = 304, Ï = 207, Ö = 214, Ş = 350, Ü = 220,

        _0 = 48, _1 = 49, _2 = 50, _3 = 51, _4 = 52, _5 = 53, _6 = 54, _7 = 55, _8 = 56, _9 = 57
    }

    [Serializable]
    public enum LetterType
    {
        Null,
        Consonant,
        Vowel,
        Numeric,
        SpecialChar,
        None
    }

    [Serializable]
    public class LetterObj
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LetterType LetterType { get; private set; } = LetterType.Null;
        [field: SerializeField] public Letter Letter { get; private set; } = Letter.Null;
        [field: SerializeField] public char UpperLetterChar { get; private set; }
        [field: SerializeField] public char LowerLetterChar { get; private set; }

        public LetterObj(Letter letter) 
        {
            Letter = letter;
            Name = letter.ToString();
            LetterType = letter switch
            {
                Letter.Null => LetterType.Null,
                Letter.None => LetterType.None,
                Letter.NewLine or Letter.Space or Letter.Underline or
                Letter.QuestionMark or Letter.ExclamationMark or Letter.Comma or Letter.Period or
                Letter.Colon or Letter.Semicolon or Letter.DoubleQuote or Letter.SingleQuote => LetterType.SpecialChar,
                Letter.A or Letter.E or Letter.É or Letter.I or Letter.O or Letter.U or Letter.İ or Letter.Ö or Letter.Ü
                or Letter.È or Letter.Ë or Letter.Ï or Letter.Ó => LetterType.Vowel,
                Letter.B or Letter.C or Letter.D or Letter.F or Letter.G or Letter.H or Letter.J or Letter.K or
                Letter.L or Letter.M or Letter.N or Letter.P or Letter.Q or Letter.R or Letter.S or Letter.T or
                Letter.V or Letter.W or Letter.X or Letter.Y or Letter.Z or Letter.Ç or Letter.Ğ or Letter.Ş => LetterType.Consonant,
                _ => LetterType.None
            };
            
            if (LetterType is LetterType.None or LetterType.Null)
            {
                UpperLetterChar = default;
                LowerLetterChar = default;
                return;
            }
            UpperLetterChar = (char)((int)(Letter));
            if(LetterType is not LetterType.SpecialChar or LetterType.Numeric)
            {
                LowerLetterChar = char.ToLower(UpperLetterChar, Thread.CurrentThread.CurrentCulture);
            }
            
        }

    }
}