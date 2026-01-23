using TACM.Core;
using TACM.UI.Models;

namespace TACM.UI.ViewModels;

public partial class ShowPicturesToMemorizeViewModel : ViewModel
{
    private readonly ushort _objectQuantity;
    private readonly HashSet<int> _indexesAlreadyPicked = [];
    private readonly Random _random = new();

    private readonly string[] _randomDrawPictures;

    private bool _canShowButtonNext = false;
    private bool _showStartText = false;
    private bool _canShowPic = true;
    private ushort _randomDrawPicturesCount;
    private string _currentPicture = "";
    private ImageSource? _currentImageSource;
    private ushort _fontSize = AppConstants.DEFAULT_WORD_TEST_FONTSIZE;

    public ushort FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    public string CurrentPicture
    {
        get => _currentPicture;
        set
        {
            _currentPicture = value;
            OnPropertyChanged(nameof(CurrentPicture));
        }
    }

    public ImageSource? CurrentImageSource
    {
        get => _currentImageSource;
        set
        {
            _currentImageSource = value;
            OnPropertyChanged(nameof(CurrentImageSource));
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

    public bool CanShowPic
    {
        get => _canShowPic;
        set
        {
            _canShowPic = value;
            OnPropertyChanged(nameof(CanShowPic));
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

    public string[] RandomDrawPictures
    {
        get => _randomDrawPictures;
    }


    public ShowPicturesToMemorizeViewModel(ushort objectQuantity) : base()
    {
        _objectQuantity = objectQuantity;

        _randomDrawPicturesCount = _objectQuantity;
        _randomDrawPictures = new string[_randomDrawPicturesCount];
        
        if (_randomDrawPicturesCount == 3)
        {
            LoadPicturesDictionary(1, 3);
            DrawRandomPicturesFromDictionary(); // Demo can remain random
        }
        else if (_randomDrawPicturesCount == 25)
        {
            LoadPicturesDictionary(7, 31);
            DrawSequentialPicturesFromDictionary(); // Preview must be sequential
        }
        else
        {
            // Default case for other quantities (e.g., demo with 10 pictures)
            LoadPicturesDictionary(1, _randomDrawPicturesCount);
            DrawRandomPicturesFromDictionary();
        }
    }


    private void UpdateCurrentPicture(in int index)
    {
        CurrentPicture = _randomDrawPictures[index];
        CurrentImageSource = GetImageSource(_randomDrawPictures[index]);
    }

    public void DrawRandomPicturesFromDictionary()
    {
        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawPictures);

        for (int i = 0; i < _randomDrawPicturesCount; i++)
        {
            _randomDrawPictures[i] = _pictures[i]; // Use sorted list in order
        }

        UpdateCurrentPicture(0);
        _indexesAlreadyPicked.Add(0);
    }


    public void DrawSequentialPicturesFromDictionary()
    {
        _indexesAlreadyPicked.Clear();
        Array.Clear(_randomDrawPictures);

        for (int i = 0; i < _randomDrawPicturesCount; i++)
        {
            _randomDrawPictures[i] = _pictures[i]; // _pictures are already sorted in LoadPicturesDictionary
        }

        UpdateCurrentPicture(0); // Start from first
        _indexesAlreadyPicked.Add(0);
    }
    public void HideCurrentPicture()
    {
        CurrentPicture = string.Empty;
    }
    public bool ToggleSequentialPictures()
    {
        if (_indexesAlreadyPicked.Count >= _randomDrawPicturesCount)
            return false;

        int nextIndex = _indexesAlreadyPicked.Count;
        _indexesAlreadyPicked.Add(nextIndex);

        UpdateCurrentPicture(nextIndex);
        return true;
    }

}
