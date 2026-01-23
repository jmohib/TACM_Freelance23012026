namespace TACM.UI.Utils;

public sealed class StartTestInitialTextsProvider
{
    public static (SpanLine[] lines, string buttonText) GetVerbalMemoryStartPageInfo(ushort wordsQuantity)
    {
        var textLines = new SpanLine[]
        {
            new($"You will be shown {wordsQuantity} words one at a time for 3 seconds. ", Colors.White),
            new(Environment.NewLine, Colors.White),
            new($"Try to remember each one. After you see all {wordsQuantity}, you will be asked to identify them among other words.", Colors.White),
            new(Environment.NewLine, Colors.White),
            new($"Click ", Colors.White),
            new($"START TEST ", Colors.LightSkyBlue, FontAttributes.Bold),
            new($"to begin. ", Colors.White),
        };

        return (textLines, "Start Test");
    }

    public static (SpanLine[] lines, string buttonText) GetNonVerbalMemoryStartPageInfo(ushort wordsQuantity)
    {
        var textLines = new SpanLine[]
        {
            new($"You will be shown {wordsQuantity} pictures one at a time for 3 seconds. ", Colors.White),
            new(Environment.NewLine, Colors.White),
            new($"Try to remember each one. After you see all {wordsQuantity}, you will be asked to identify them among other pictures.", Colors.White),
            new(Environment.NewLine, Colors.White),
            new($"Click ", Colors.White),
            new($"START TEST ", Colors.LightSkyBlue, FontAttributes.Bold),
            new($"to begin. ", Colors.White),
        };

        return (textLines, "Start Test");
    }

    public static (SpanLine[] lines, string buttonText) GetAttentionTestStartPageInfo(char target)
    {
        var textLines = new SpanLine[]
        {
            new("Press the space bar as quickly as you can when you see the letter ", Colors.White),
            new(target.ToString(), Colors.LightSkyBlue, FontAttributes.Bold),
            new("." + Environment.NewLine, Colors.White),
            new("Do not press the space bar for any other letter." + Environment.NewLine, Colors.White),
        };

        return (textLines, "Start Test");
    }
}
