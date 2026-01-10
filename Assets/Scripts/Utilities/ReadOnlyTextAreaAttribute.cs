using UnityEngine;

public class ReadOnlyTextAreaAttribute : PropertyAttribute
{
    public int minLines;
    public int maxLines;

    public ReadOnlyTextAreaAttribute(int minLines = 3, int maxLines = 10)
    {
        this.minLines = minLines;
        this.maxLines = maxLines;
    }
}
