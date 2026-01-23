using System.Windows.Input;
using TACM.Core;
using TACM.Data.DbContextEntitiesExtensions;
using TACM.UI.Models;
using TACM.UI.Utils;

namespace TACM.UI.ViewModels;

public partial class PictureTestMemoryViewModel : ViewModel
{
    private ushort _correctAnswersCount = 0;
    private ushort _wrongAnswersCount = 0;
    private ushort _fontSize = AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
    private readonly HashSet<int> _indexesAlreadyPicked = [];        
    private readonly DrawRigthAndWrongAnswersPair[] _randomDrawAnswersPairs;
    private readonly string[] _correctAnswers;
    private readonly Random _random = new();
    private ushort _randomDrawAnswersCount;
    private DrawRigthAndWrongAnswersPair _currentAnswersPair;
    private AnswerState _answerState = AnswerState.Undefined;
    private bool _answerWasChosen = false;
    private bool _showCorrectAnswerLabel = false;
    private bool _showIncorrectAnswerLabel = false;
    private short _currentAnswersPairIndex = -1;

    private ImageSource? _currentImageSource1;
    private ImageSource? _currentImageSource2;


    public ICommand AnswerSelectedCommand { get; set; }
    public ICommand NextAnswersPairCommand { get; set; }
    public int TestTryNumber { get; set; } = 0;
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

    public ImageSource? CurrentImageSource1
    {
        get => _currentImageSource1;
        set
        {
            _currentImageSource1 = value;
            OnPropertyChanged(nameof(CurrentImageSource1));
        }
    }

    public ImageSource? CurrentImageSource2
    {
        get => _currentImageSource2;
        set
        {
            _currentImageSource2 = value;
            OnPropertyChanged(nameof(CurrentImageSource2));
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
            _showCorrectAnswerLabel= value;
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


    public PictureTestMemoryViewModel(ushort objectQuantity, string[] correctAnswers) : base() 
    {
        _randomDrawAnswersCount = objectQuantity;
        _randomDrawAnswersPairs = new DrawRigthAndWrongAnswersPair[_randomDrawAnswersCount];
        _correctAnswers = correctAnswers;

        AnswerSelectedCommand = new Command(HandletSelectedAnswer);
        NextAnswersPairCommand = new Command(SelectNextWordsPair);
    }


    public async Task StartTestAsync(bool preview)
    {
        if (TestTryNumber == 0 && preview) // Demo
        {
            _randomDrawAnswersCount = 3;
            LoadPicturesDictionary(1, 6);
            DrawSequentialAnswersPairs(); // 1–3 with 4–6
        }
        else if (TestTryNumber == 1 || TestTryNumber == 2)
        {
            if (TestTryNumber == 1)
            {
                SessionTry.Set("isFirstNonVerbalCompleted", true);
                SessionTry.Set("isSecondNonVerbalCompleted", false);
            }
            else if (TestTryNumber == 2)
            {
                SessionTry.Set("isFirstNonVerbalCompleted", false);
                SessionTry.Set("isSecondNonVerbalCompleted", true);
            }

            _randomDrawAnswersCount = 25;
            LoadPicturesDictionary(7, 81); // All test pictures
            DrawSequentialAnswersPairs(); // 7–31 paired with 32–56 or 57–81
        }
        else if (!preview) // Preview (when TestTryNumber is not 0–2)
        {
            _randomDrawAnswersCount = 25;
            LoadPicturesDictionary(7, 31);
            DrawPreviewImagesInSequence();
        }
        SessionTry.Set("CurrentTestType", "non-verbal");
        var testResult = await TacmDbContext.SaveTestResultAsync(new Entities.TestResult
        {
            Id = 0,
            ItemsCount = _randomDrawAnswersCount,
            Preview = preview,
            TestType = AppConstants.NON_VERBAL_MEMORY_TEST_TYPE,
            SessionId = SessionCollectedData.GetCollectedSessionId(),
            IsDeleted = false
        });

        SessionCollectedData.CollectTestResultId(testResult.Id);
        Start = DateTime.UtcNow;
    }


    public void DrawPreviewImagesInSequence()
    {
        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawAnswersPairs);

        int previewStart = 7;

        for (int i = 0; i < _randomDrawAnswersCount; i++)
        {
            string imageName = $"{previewStart + i}.png";

            _randomDrawAnswersPairs[i] = new DrawRigthAndWrongAnswersPair
            {
                Answer1 = imageName,
                Answer2 = "", // Leave blank; we’ll only show Answer1 in UI
                CorrectAnwserNumber = 1 // Not used in preview
            };
        }
        SessionTry.Set("CurrentTestType", "non-verbal");
        _currentAnswersPairIndex = 0;
        _indexesAlreadyPicked.Add(0);
        UpdateCurrentAnswersPair(_currentAnswersPairIndex);
    }


    private void UpdateCurrentAnswersPair(in int index)
    {
        CurrentAnswersPair = _randomDrawAnswersPairs[index];
        CurrentImageSource1 = GetImageSource(CurrentAnswersPair.Answer1);
        CurrentImageSource2 = GetImageSource(CurrentAnswersPair.Answer2);
    }
    public void DrawSequentialAnswersPairs()
    {
        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawAnswersPairs);

        int pairCount = _randomDrawAnswersCount;

        int correctStart, wrongStart;

        switch (TestTryNumber)
        {
            case 0: // Demo: 1–3 with 4–6
                correctStart = 1;
                wrongStart = 4;
                break;
            case 1: // Test Try #1: 7–31 with 32–56
                correctStart = 7;
                wrongStart = 32;
                break;
            case 2: // Test Try #2: 7–31 with 57–81
                correctStart = 7;
                wrongStart = 57;
                break;
            default:
                throw new InvalidOperationException("Invalid TestTryNumber value.");
        }

        for (int i = 0; i < pairCount; i++)
        {
            string correctImage = $"{correctStart + i}.png";
            string wrongImage = $"{wrongStart + i}.png";

            int correctPos = _random.Next(0, 2); // Randomly place correct image

            if (correctPos == 0)
            {
                _randomDrawAnswersPairs[i] = new DrawRigthAndWrongAnswersPair
                {
                    Answer1 = correctImage,
                    Answer2 = wrongImage,
                    CorrectAnwserNumber = 1
                };
            }
            else
            {
                _randomDrawAnswersPairs[i] = new DrawRigthAndWrongAnswersPair
                {
                    Answer1 = wrongImage,
                    Answer2 = correctImage,
                    CorrectAnwserNumber = 2
                };
            }
        }

        _currentAnswersPairIndex = 0;
        _indexesAlreadyPicked.Add(0);
        UpdateCurrentAnswersPair(_currentAnswersPairIndex);
    }

    public void DrawPreviewOnlyAnswers()
    {
        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawAnswersPairs);

        for (int i = 0; i < 25; i++)
        {
            string correctImage = $"{7 + i}.png";

            _randomDrawAnswersPairs[i] = new DrawRigthAndWrongAnswersPair
            {
                Answer1 = correctImage,
                Answer2 = "", // Not used
                CorrectAnwserNumber = 1
            };
        }

        _currentAnswersPairIndex = 0;
        _indexesAlreadyPicked.Add(0);
        UpdateCurrentAnswersPair(_currentAnswersPairIndex);
    }


    public void DrawRandomAnswersPairsFromDictionary()
    {
        var random = new Random();

        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawAnswersPairs);

        ushort count = 0;
        var correctAnswersShuffleBase = _correctAnswers.AsSpan();
        var wrongAnswerShuffleBase = _pictures.Except(_correctAnswers).ToArray().AsSpan();

        random.Shuffle(correctAnswersShuffleBase);
        random.Shuffle(wrongAnswerShuffleBase);

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

        UpdateCurrentAnswersPair(0);

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

        UpdateCurrentAnswersPair(_currentAnswersPairIndex);

        AnswerWasChosen = false;
        AnswerState = AnswerState.Undefined;
        ShowCorrectAnswerLabel = false;
        ShowIncorrectAnswerLabel = false;
        Start = DateTime.UtcNow;
    }
}
