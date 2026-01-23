using TACM.Core;
using TACM.Data;
using TACM.Data.DbContextEntitiesExtensions;
using TACM.Entities;
using TACM.UI.Models;
using TACM.UI.Utils;

namespace TACM.UI.ViewModels;

public struct AttentionTestAnswer
{
    public byte TrialNumber;
    public char Letter;
    public char Target;
    public ushort LetterNumber;
    public DateTime Start;
    public DateTime End;
    public bool Missing;
}

public struct AttentionTrialInterval
{
    public byte TrialNumber { get; private set; }
    public ushort TrialCount { get; private set; }
    public byte Start {  get; private set; }
    public byte End { get; private set; }

    public AttentionTrialInterval(byte trialNumber, ushort trialCount)
    {        
        TrialNumber = trialNumber;
        TrialCount = trialCount;

        Start = (byte)((trialCount * trialNumber) - trialCount);
        End = (byte)(trialCount * trialNumber);
    }
}

public partial class AttentionTestViewModel : ViewModel
{
    private bool _isDemo = false;
    private ushort _trialsCount;
    private ushort _currentAnswerCount = 0;
    private ushort _missingAnswersCount = 0;
    private char _currentLetter;
    private byte _currentTrialNumber = 1;
    private char _target;
    private ushort _fontSize;
    private bool _answered = false;
    private bool _canShowNextButton = false;
    private bool _canShowLetter = false;
    private Color _letterColor = Colors.White;
    private Random _random = new(10);

    private AttentionTrialInterval _t1Interval;
    private AttentionTrialInterval _t2Interval;
    private AttentionTrialInterval _t3Interval;
    private AttentionTrialInterval _t4Interval;

    private DateTime _currentTime = DateTime.UtcNow;
    private Stack<AttentionTestAnswer> _answers = new();


    public char CurrentLetter 
    { 
        get => _currentLetter; 
        private set
        {
            _currentLetter = value;
            OnPropertyChanged(nameof(CurrentLetter));
        }
    }

    public byte CurrentTrialNumber
    {
        get => _currentTrialNumber;
        private set
        {
            _currentTrialNumber = value;
            OnPropertyChanged(nameof(CurrentTrialNumber));
        }
    }

    public DateTime CurrentTime
    {
        get => _currentTime;
        private set
        {
            _currentTime = value;
            OnPropertyChanged(nameof(CurrentTime));
        }
    }

    public ushort TrialsCount
    {
        get => _trialsCount;
        private set
        {
            _trialsCount = value;
            OnPropertyChanged(nameof(TrialsCount));
        }
    }

    public ushort FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    public bool Answered
    {
        get => _answered;
        private set
        {
            _answered = value;
            OnPropertyChanged(nameof(Answered));
        }
    }

    public char Target
    {
        get => _target;
        private set
        {
            _target = value;
            OnPropertyChanged(nameof(Target));
        }
    }

    public bool CanShowButtonNext
    {
        get => _canShowNextButton;
        set
        {
            _canShowNextButton = value;
            OnPropertyChanged(nameof(CanShowButtonNext));
        }
    }

    public bool CanShowLetter
    {
        get => _canShowLetter;
        set
        {
            _canShowLetter = value;
            OnPropertyChanged(nameof(CanShowLetter));

            LetterColor = _canShowLetter ? Colors.White : Colors.Transparent;
        }
    }

    public Color LetterColor
    {
        get => _letterColor;
        set
        {
            _letterColor = value;
            OnPropertyChanged(nameof(LetterColor));
        }
    }


    public AttentionTestViewModel(char target, ushort trialsCount, bool isDemo = false)
    {
        _isDemo = isDemo;
        _target = target;
        _trialsCount = trialsCount;

        _t1Interval = new AttentionTrialInterval(1, trialsCount);
        _t2Interval = new AttentionTrialInterval(2, trialsCount);
        _t3Interval = new AttentionTrialInterval(3, trialsCount);
        _t4Interval = new AttentionTrialInterval(4, trialsCount);
    }

    
    private byte GetCurrentTrial(ushort answerCount)
    {
        if (answerCount >= _t1Interval.Start && answerCount <= _t1Interval.End)
            return 1;

        if (answerCount >= _t2Interval.Start && answerCount <= _t2Interval.End)
            return 2;

        if (answerCount >= _t3Interval.Start && answerCount <= _t3Interval.End)
            return 3;

        if (answerCount >= _t4Interval.Start && answerCount <= _t4Interval.End)
            return 4;
        
        return 0;
    }

    public void DrawNewLetter()
    {
        CanShowLetter = false;

        CurrentTime = DateTime.UtcNow;

        int[] randomIndexes = [
            _random.Next(0, 5),
            _random.Next(6, 11),
            _random.Next(12, 18),
            _random.Next(19, 26),
            _random.Next(27, 36)
        ];

        var randomIndex = _random.Next(0, randomIndexes.Length);
        var randomTarget = _random.Next(0, 3);
        var randomLetter = randomTarget == 1 ? _target : AppConstants.ALPHABET_AND_NUMBERS[randomIndexes[randomIndex]];

        CurrentLetter = randomLetter;
        Answered = false;
        CanShowLetter = true;
        _currentAnswerCount++;
    }

    public void RegisterAnswer(bool missing = false)
    {
        CurrentTrialNumber = GetCurrentTrial(_currentAnswerCount);

        _answers.Push(new AttentionTestAnswer
        {
            Letter = _currentLetter,
            Target = _target,
            LetterNumber = _currentAnswerCount,
            Missing = missing,
            TrialNumber = CurrentTrialNumber,
            Start = _currentTime,
            End = DateTime.UtcNow
        });

        Answered = !missing;

        if (missing)
            _missingAnswersCount++;

        SessionTry.Set("CurrentTestType", "attention");
    }

    public async Task SaveTestResultsAsync()
    {
        await Task.Run(async () =>
        {
            var context = TacmDbContextFactory.CreateDbContext();

            var entity = new TestResult
            {
                Id = 0,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                ItemsCount = Convert.ToUInt16(_answers.Count),
                SessionId = SessionCollectedData.GetCollectedSessionId(),
                TestType = AppConstants.ATTENTION_TEST_TYPE,
                Preview = _isDemo,
                End = DateTime.UtcNow,

                Items = _answers.Select(_ => new TestResultItem
                {
                    Id = 0,
                    Answer = _.Letter,
                    Item1 = _target.ToString(),
                    Item2 = null,
                    IsCorrect = _.Letter == _target && !_.Missing,
                    IsDeleted = false,
                    Start = _.Start,
                    End = _.End,
                    CreatedAt = DateTime.UtcNow,
                    SessionId = SessionCollectedData.GetCollectedSessionId()
                })
                .ToHashSet()
            };

            await context.SaveTestResultAsync(entity);
        });
    }
}
