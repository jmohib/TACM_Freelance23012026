using TACM.Core;

namespace TACM.UI.Pages;

public partial class RememberFollowingObjectsPage : ContentPage
{
    private readonly ushort _objectQuantity;
    private readonly string _objectType;

    private readonly IDictionary<string, ContentPage> _pageToRedirectRegardingObjectType;

	public RememberFollowingObjectsPage(ushort objectQuantity, string objectType)
	{
		InitializeComponent();
            
        _objectQuantity = objectQuantity;
        _objectType = objectType;

        _pageToRedirectRegardingObjectType = new Dictionary<string, ContentPage>()
        {
            { AppConstants.OBJECT_TYPE_WORDS_ON_PLURAL, new ShowWordsToMemorizePage(_objectQuantity) },
            { AppConstants.OBJECT_TYPE_PICTURES_ON_PLURAL, new ShowPicturesToMemorizePage(_objectQuantity) }
        };

        BuildStartPageSpanText();
    }

    private void BuildStartPageSpanText()
    {
        Span[] spanLines = [
            new Span()
            {
                Text = $"Remember the following {_objectQuantity} {_objectType}.",
                TextColor = Colors.White
            },

            new Span() { Text = Environment.NewLine },
            new Span() { Text = "Click ", TextColor = Colors.White },
            new Span() { Text = "NEXT ", TextColor = Colors.LightSkyBlue, FontAttributes = FontAttributes.Bold },
            new Span() { Text = "to begin.", TextColor = Colors.White }
        ];

        lblRememberText.FormattedText.Spans.Clear();

        foreach (var line in spanLines)
            lblRememberText.FormattedText.Spans.Add(line);
    }

    public async void BtnStartPage(object sender, EventArgs e)
	{
		//await Navigation.PushAsync(_pageToRedirectRegardingObjectType[_objectType], true);
        Application.Current.MainPage = new NavigationPage(_pageToRedirectRegardingObjectType[_objectType]);
    }
}