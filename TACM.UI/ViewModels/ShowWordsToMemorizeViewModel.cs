using TACM.Core;
using TACM.UI.Models;
using TACM.UI.Utils;

namespace TACM.UI.ViewModels;

public partial class ShowWordsToMemorizeViewModel : ViewModel
{
    private readonly ushort _objectQuantity;
    private readonly HashSet<int> _indexesAlreadyPicked = [];
    private readonly Random _random = new();
    private int _currentIndex = 0;
    private readonly string[] _randomDrawWords;
    private bool _showStartText = false;
    private bool _canShowButtonNext = false;
    private bool _canShowWord = true;
    private ushort _randomDrawWordsCount;
    private string _currentWord = "";
    private ushort _fontSize = AppConstants.DEFAULT_WORD_TEST_FONTSIZE;
    private List<string> wordsToRemember = new List<string>();
    private List<string> distractingWords = new List<string>();
    public ushort FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    public string CurrentWord
    {
        get => _currentWord;
        set
        {
            _currentWord = value;
            OnPropertyChanged(nameof(CurrentWord));
        }
    }
    public bool ShowStartText
    {
        get => _showStartText;
        set
        {
            _showStartText = value;
            OnPropertyChanged(nameof(ShowStartText));
        }
    }
    public bool CanShowWord
    {
        get => _canShowWord;
        set
        {
            _canShowWord = value;
            OnPropertyChanged(nameof(CanShowWord));
        }
    }
    public bool CanShowButtonNext
    {
        get => _canShowButtonNext;
        set
        {
            _canShowButtonNext = value;
            OnPropertyChanged(nameof(CanShowButtonNext));
        }
    }

    public string[] RandomDrawWords
    {
        get => _randomDrawWords;
    }


    public ShowWordsToMemorizeViewModel(ushort objectQuantity) : base()
    {
        _objectQuantity = objectQuantity;

        _randomDrawWordsCount = _objectQuantity;
        _randomDrawWords = new string[_randomDrawWordsCount];

        LoadWordsDictionary();
        DrawRandomWordsFromDictionary();
    }


    public void DrawRandomWordsFromDictionary()
    {
        AppLogger.Log($"DrawRandomWordsFromDictionary called - _randomDrawWordsCount: {_randomDrawWordsCount}, _words.Count: {_words.Count}");
        
        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawWords);

        ushort count = 0;
       
        if (_randomDrawWordsCount == 3) {
            wordsToRemember = _words.Take(3).ToList();
            distractingWords = _words.Skip(3).Take(3).ToList();
        }
        else if (_randomDrawWordsCount == 25 && !SessionTry.Get<bool>("isFirstVerbalCompleted"))
        {
            wordsToRemember = _words.Skip(6).Take(25).ToList();
            distractingWords = _words.Skip(31).Take(25).ToList();
            //SessionTry.Set("isFirstVerbalCompleted", true);
        }
        else if ( _randomDrawWordsCount == 25 && SessionTry.Get<bool>("isFirstVerbalCompleted") )
        {
            wordsToRemember = _words.Skip(6).Take(25).ToList();
            distractingWords = _words.Skip(56).Take(25).ToList();
        }
        else
        {
            // Default case for any other quantity (e.g., demo with 10 words)
            AppLogger.Log($"Using default word selection for quantity: {_randomDrawWordsCount}");
            wordsToRemember = _words.Take(_randomDrawWordsCount).ToList();
            distractingWords = _words.Skip(_randomDrawWordsCount).Take(_randomDrawWordsCount).ToList();
        }

        AppLogger.Log($"wordsToRemember.Count: {wordsToRemember?.Count ?? 0}");
        AppLogger.Log($"distractingWords.Count: {distractingWords?.Count ?? 0}");

        // Safety check: ensure we have enough words
        if (wordsToRemember == null || wordsToRemember.Count < _randomDrawWordsCount)
        {
            var errorMsg = $"Not enough words to remember. Have {wordsToRemember?.Count ?? 0}, need {_randomDrawWordsCount}. Total available: {_words.Count}";
            AppLogger.Log($"ERROR: {errorMsg}");
            throw new InvalidOperationException(errorMsg);
        }

        while (count < _randomDrawWordsCount)
        {
            if (count >= wordsToRemember.Count)
            {
                AppLogger.Log($"ERROR: count ({count}) >= wordsToRemember.Count ({wordsToRemember.Count})");
                break;
            }
            
            var word = wordsToRemember[count];

            if (Array.BinarySearch(_randomDrawWords, word) >= 0)
                continue;
            
            _randomDrawWords[count] = word;
            count++;
        }

        CurrentWord = _randomDrawWords[0];
        _indexesAlreadyPicked.Add(0);
    }
    public void HideCurrentWord()
    {
        CurrentWord = string.Empty;
    }

    //public bool ToggleRandomDrawnWords()
    //{
    //    if(_indexesAlreadyPicked.Count >= _randomDrawWordsCount)
    //        return false;

    //    bool found = false;

    //    do
    //    {
    //        var index = _random.Next(0, _randomDrawWordsCount);

    //        if (_indexesAlreadyPicked.Contains(index))
    //            continue;

    //        _indexesAlreadyPicked.Add(index);
    //        CurrentWord = _randomDrawWords[index];                
    //        found = true;
    //    }
    //    while (!found);

    //    return true;
    //}

    public bool ToggleRandomDrawnWords()
    {
        if (_indexesAlreadyPicked.Count >= _randomDrawWordsCount)
            return false;

        if (_currentIndex >= _randomDrawWordsCount)
            return false;

        _indexesAlreadyPicked.Add(_currentIndex);
        CurrentWord = _randomDrawWords[_currentIndex];
        _currentIndex++;

        return true;
    }
}
