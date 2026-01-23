using System.Windows.Input;
using TACM.Core;
using TACM.Data.DbContextEntitiesExtensions;
using TACM.UI.Models;
using TACM.UI.Utils;

namespace TACM.UI.ViewModels;

public partial class WordTestMemoryViewModel : ViewModel
{
    private ushort _correctAnswersCount = 0;
    private ushort _wrongAnswersCount = 0;
    private ushort _fontSize = AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
    private readonly HashSet<int> _indexesAlreadyPicked = [];
    private readonly DrawRigthAndWrongAnswersPair[] _randomDrawAnswersPairs;
    private readonly string[] _correctAnswers;
    private ushort _randomDrawAnswersCount;
    private DrawRigthAndWrongAnswersPair _currentAnswersPair;
    private AnswerState _answerState = AnswerState.Undefined;
    private bool _answerWasChosen = false;
    private bool _showCorrectAnswerLabel = false;
    private bool _showIncorrectAnswerLabel = false;
    private short _currentAnswersPairIndex = -1;

    public ICommand AnswerSelectedCommand { get; set; }
    public ICommand NextAnswersPairCommand { get; set; }

    public ushort FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    public DrawRigthAndWrongAnswersPair CurrentAnswersPair
    {
        get => _currentAnswersPair;
        set
        {
            _currentAnswersPair = value;
            OnPropertyChanged(nameof(CurrentAnswersPair));
        }
    }

    public AnswerState AnswerState
    {
        get => _answerState;
        set
        {
            _answerState = value;
            OnPropertyChanged(nameof(AnswerState));
        }
    }

    public bool AnswerWasChosen
    {
        get => _answerWasChosen;
        set
        {
            _answerWasChosen = value;
            OnPropertyChanged(nameof(AnswerWasChosen));
        }
    }

    public bool ShowCorrectAnswerLabel
    {
        get => _showCorrectAnswerLabel;
        set
        {
            _showCorrectAnswerLabel = value;
            OnPropertyChanged(nameof(ShowCorrectAnswerLabel));
        }
    }

    public bool ShowIncorrectAnswerLabel
    {
        get => _showIncorrectAnswerLabel;
        set
        {
            _showIncorrectAnswerLabel = value;
            OnPropertyChanged(nameof(ShowIncorrectAnswerLabel));
        }
    }

    public bool AllAnswersPairsWereVisited { get; set; } = false;

    public ushort CorrectAnswersCount { get => _correctAnswersCount; }
    public ushort WrongAnswerCount { get => _wrongAnswersCount; }
    public DateTime Start { get; private set; } = DateTime.UtcNow;


    public WordTestMemoryViewModel(ushort objectQuantity, string[] correctAnswers) : base()
    {
        _randomDrawAnswersCount = objectQuantity;
        _randomDrawAnswersPairs = new DrawRigthAndWrongAnswersPair[_randomDrawAnswersCount];
        _correctAnswers = correctAnswers;

        AnswerSelectedCommand = new Command(HandletSelectedAnswer);
        NextAnswersPairCommand = new Command(SelectNextWordsPair);
    }


    public async Task StartTestAsync(bool preview)
    
    {
        LoadWordsDictionary();
        DrawRandomAnswersPairsFromDictionary();

        var testResult = await TacmDbContext.SaveTestResultAsync(new Entities.TestResult
        {
            Id = 0,
            ItemsCount = _randomDrawAnswersCount,
            Preview = preview,
            TestType = AppConstants.VERBAL_MEMORY_TEST_TYPE,
            SessionId = SessionCollectedData.GetCollectedSessionId(),
            IsDeleted = false
        });

        SessionCollectedData.CollectTestResultId(testResult.Id);
        Start = DateTime.UtcNow;
    }

    public void DrawRandomAnswersPairsFromDictionary()
    {
        var random = new Random();

        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawAnswersPairs);

        ushort count = 0;
        var correctAnswersShuffleBase = _correctAnswers.AsSpan();
        // var wrongAnswerShuffleBase = _words.Except(_correctAnswers).ToArray().AsSpan();
        var wrongAnswerShuffleBase = new List<string>();
        if (_randomDrawAnswersCount == 3)
            wrongAnswerShuffleBase = _words.Skip(3).Take(3).ToList();
        else if (_randomDrawAnswersCount == 25 && !SessionTry.Get<bool>("isFirstVerbalCompleted"))
        {
            wrongAnswerShuffleBase = _words.Skip(31).Take(25).ToList();
            SessionTry.Set("isFirstVerbalCompleted", true);
            SessionTry.Set("isSecondVerbalCompleted", false);
        }
        else if (_randomDrawAnswersCount == 25 && SessionTry.Get<bool>("isFirstVerbalCompleted"))
        {
            wrongAnswerShuffleBase = _words.Skip(56).Take(25).ToList();
            SessionTry.Set("isSecondVerbalCompleted", true);
            SessionTry.Set("isFirstVerbalCompleted", false);
        }

        SessionTry.Set("CurrentTestType", "verbal");
        //  random.Shuffle(correctAnswersShuffleBase);
        // random.Shuffle(wrongAnswerShuffleBase);

        while (count < _randomDrawAnswersCount)
        {            
            var rightAnswer = correctAnswersShuffleBase[count];
            var wrongAnswer = wrongAnswerShuffleBase[count];
            var rightWordPos = random.Next(0, 3);

            if (rightWordPos == 1)
            {
                _randomDrawAnswersPairs[count].Answer1 = rightAnswer;
                _randomDrawAnswersPairs[count].Answer2 = wrongAnswer;
                _randomDrawAnswersPairs[count].CorrectAnwserNumber = 1;
            }
            else
            {
                _randomDrawAnswersPairs[count].Answer1 = wrongAnswer;
                _randomDrawAnswersPairs[count].Answer2 = rightAnswer;
                _randomDrawAnswersPairs[count].CorrectAnwserNumber = 2;
            }

            count++;
        }

        CurrentAnswersPair = _randomDrawAnswersPairs[0];

        _currentAnswersPairIndex = 0;
        _indexesAlreadyPicked.Add(0);
    }

    public async void HandletSelectedAnswer(object data)
    {
        var answer = Convert.ToByte(data);

        AnswerWasChosen = true;
        AnswerState = CurrentAnswersPair.CorrectAnwserNumber == answer ? AnswerState.Correct : AnswerState.Incorrect;
        ShowCorrectAnswerLabel = AnswerState == AnswerState.Correct;
        ShowIncorrectAnswerLabel = AnswerState == AnswerState.Incorrect;

        switch (AnswerState)
        {
            case AnswerState.Correct:
                _correctAnswersCount++;
                break;
            case AnswerState.Incorrect:
                _wrongAnswersCount++;
                break;
        }

        SessionCollectedData.CollectAnswersCount(_correctAnswersCount, _wrongAnswersCount);

        await TacmDbContext.SaveTestResultItemAsync(new Entities.TestResultItem
        {
            Id = 0,
            SessionId = SessionCollectedData.GetCollectedSessionId(),
            TestResultId = SessionCollectedData.GetCollectedTestResultId(),

            Item1 = CurrentAnswersPair.Answer1,
            Item2 = CurrentAnswersPair.Answer2,
            Answer = Convert.ToChar(answer),

            IsCorrect = ShowCorrectAnswerLabel,
            IsDeleted = false,
            Start = Start,
            End = DateTime.UtcNow
        });
    }

    public async void SelectNextWordsPair()
    {
        AllAnswersPairsWereVisited = false;
        _currentAnswersPairIndex++;

        if (_currentAnswersPairIndex >= _randomDrawAnswersPairs.Length)
        {
            AllAnswersPairsWereVisited = true;
            await TacmDbContext.UpdateTestResultEndDateAsync(SessionCollectedData.GetCollectedTestResultId(), DateTime.UtcNow);
            return;
        }

        CurrentAnswersPair = _randomDrawAnswersPairs[_currentAnswersPairIndex];
        AnswerWasChosen = false;
        AnswerState = AnswerState.Undefined;
        ShowCorrectAnswerLabel = false;
        ShowIncorrectAnswerLabel = false;
        Start = DateTime.UtcNow;
    }
    public async Task<int> GetCorrectAnswerCount()
    {
        return await TacmDbContext.GetCorrectAnswerCountAsync(SessionCollectedData.GetCollectedTestResultId());
    }
}
