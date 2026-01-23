namespace TACM.Core;

public struct DrawRigthAndWrongAnswersPair
{
    public string Answer1 { get; set; }
    public string Answer2 { get; set; }
    public byte CorrectAnwserNumber { get; set; }

    public override readonly string ToString()
    {
        return $"[1]: {Answer1} - [2]: {Answer2} - Correct: {CorrectAnwserNumber}";
    }
}
