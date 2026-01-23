//using AVKit;

namespace TACM.UI.Models;

public class SettingsModel : Model
{
    private int _id = 0;
    private char? _target;
    private ushort? _fontSize;
    private ushort? _t1;
    private ushort? _t2;
    private ushort? _t3;
    private ushort? _goProbability;
    private ushort? _noProbability;
    private ushort? _trials;
    private ushort? _rnd;
    private ushort? _verbalMemoryTestWordsQuantity { get; set; }
    private ushort? _nonVerbalMemoryTestWordsQuantity { get; set; }
    private string? _appTitle {  get; set; }

    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged(nameof(Id));
        }
    }

    public char? Target 
    { 
        get => _target; 
        set
        {
            _target = value;
            OnPropertyChanged(nameof(Target));
        }
    }

    public ushort? FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            OnPropertyChanged(nameof(FontSize));
        }
    }

    public ushort? T1
    {
        get => _t1;
        set
        {
            _t1 = value;
            OnPropertyChanged(nameof(T1));
        }
    }

    public ushort? T2
    {
        get => _t2;
        set
        {
            _t2 = value;
            OnPropertyChanged(nameof(T2));
        }
    }

    public ushort? T3
    {
        get => _t3;
        set
        {
            _t3 = value;
            OnPropertyChanged(nameof(T3));
        }
    }

    public ushort? GoProbability
    {
        get => _goProbability;
        set
        {
            _goProbability = value;
            OnPropertyChanged(nameof(GoProbability));
        }
    }

    public ushort? NoProbability
    {
        get => _noProbability;
        set
        {
            _noProbability = value;
            OnPropertyChanged(nameof(NoProbability));
        }
    }

    public ushort? Trials
    {
        get => _trials;
        set
        {
            _trials = value;
            OnPropertyChanged(nameof(Trials));
        }
    }

    public ushort? RND
    {
        get => _rnd;
        set
        {
            _rnd = value;
            OnPropertyChanged(nameof(RND));
        }
    }

    public ushort? VerbalMemoryTestWordsQuantity
    {
        get => _verbalMemoryTestWordsQuantity;
        set
        {
            _verbalMemoryTestWordsQuantity = value;
            OnPropertyChanged(nameof(VerbalMemoryTestWordsQuantity));
        }
    }

    public ushort? NonVerbalMemoryTestWordsQuantity
    {
        get => _nonVerbalMemoryTestWordsQuantity;
        set
        {
            _nonVerbalMemoryTestWordsQuantity = value;
            OnPropertyChanged(nameof(NonVerbalMemoryTestWordsQuantity));
        }
    }

    public string? AppTitle
    {
        get => _appTitle;
        set
        {
            _appTitle = value;
            OnPropertyChanged(nameof(AppTitle));
        }
    }


    public SettingsModel() : base() { }

    public static SettingsModel GetDefaultSettings()
    {
        return new SettingsModel
        {
            FontSize = 72,
            GoProbability = 20,
            NonVerbalMemoryTestWordsQuantity = 25,
            NoProbability = 80,
            RND = 3500,
            T1 = 2500,
            T2 = 1500,
            T3 = 3500,
            Target = 'X',
            Trials = 50,
            VerbalMemoryTestWordsQuantity = 25,
            AppTitle = "Test of Verbal, Non Verbal",
        };
    }
}
