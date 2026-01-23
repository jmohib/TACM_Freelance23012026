using TACM.Core;

namespace TACM.Entities;

public class Settings : Entity<int>
{
    public char? Target {  get; set; }
    public ushort? FontSize { get; set; }
    public ushort? T1 { get; set; }
    public ushort? T2 { get; set; }
    public ushort? T3 { get; set; }
    public ushort? T4 { get; set; }
    public ushort? GoProbability { get; set; }
    public ushort? NoProbability { get; set; }
    public ushort? Trials { get; set; }
    public ushort? RND { get; set; }
    public string? AppTitle { get; set; }
    public ushort? VerbalMemoryTestDemoWordsQuantity {  get; set; }
    public ushort? VerbalMemoryTestWordsQuantity { get; set; }
    public ushort? NonVerbalMemoryTestWordsQuantity { get; set; }
    public ushort? NonVerbalMemoryTestDemoWordsQuantity { get; set; }


    public ushort GetVerbalMemoryTestWordsQuantity(bool forReal = true)
    {
        return forReal
            ? VerbalMemoryTestWordsQuantity ?? AppConstants.DEFAULT_VERBAL_MEMORY_WORDS_QUANTITY
            : VerbalMemoryTestDemoWordsQuantity ?? AppConstants.DEFAULT_VERBAL_MEMORY_DEMO_WORDS_QUANTITY;
    }

    public ushort GetNonVerbalMemoryTestWordsQuantity(bool forReal = true)
    {
        return forReal
            ? NonVerbalMemoryTestWordsQuantity ?? TACM.Core.AppConstants.DEFAULT_NONVERBAL_MEMORY_WORDS_QUANTITY
            : NonVerbalMemoryTestDemoWordsQuantity ?? AppConstants.DEFAULT_NONVERBAL_MEMORY_DEMO_WORDS_QUANTITY;
    }
}
