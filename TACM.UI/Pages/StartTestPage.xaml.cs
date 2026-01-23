namespace TACM.UI.Pages;

public partial class StartTestPage : ContentPage
{
	private readonly ushort _objectQuantity;
	private readonly string _objectType;

	public StartTestPage(ushort objectQuantity, string objectType)
	{
		InitializeComponent();

		_objectQuantity = objectQuantity;
		_objectType = objectType;

		BuildStartPageSpanText();
	}

	private void BuildStartPageSpanText()
	{
		Span[] spanLines = [
			new Span()
			{
				Text = $"You're now ready to begin. Try to remember the following {_objectQuantity} {_objectType}.",
				TextColor = Colors.White
			},

			new Span() { Text = Environment.NewLine },
			new Span() { Text = "Click ", TextColor = Colors.White },
			new Span() { Text = "NEXT ", TextColor = Colors.LightSkyBlue, FontAttributes = FontAttributes.Bold },
            new Span() { Text = "to begin.", TextColor = Colors.White }
        ];

		lblStartTestText.FormattedText.Spans.Clear();

		foreach (var line in spanLines)
			lblStartTestText.FormattedText.Spans.Add(line);
    }

	public async void BtnStartTestClicked(object sender, EventArgs e)
	{
        Application.Current.MainPage = new NavigationPage(new RememberFollowingObjectsPage(_objectQuantity, _objectType));
        //await Navigation.PushAsync(new RememberFollowingObjectsPage(_objectQuantity, _objectType), true);
	}
}