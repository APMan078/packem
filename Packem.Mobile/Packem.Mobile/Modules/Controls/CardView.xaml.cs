using Syncfusion.XForms.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CardView : SfCardView
    {
        public static readonly BindableProperty TapCommandProperty =
            BindableProperty.Create("TapCommand", typeof(ICommand), typeof(CardView), null);

        public ICommand TapCommand
        {
            get { return (ICommand)GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public static readonly BindableProperty CardImageColorProperty =
            BindableProperty.Create("CardImageColor", typeof(Color), typeof(CardView), null);

        public Color CardImageColor
        {
            get { return (Color)GetValue(CardImageColorProperty); }
            set { SetValue(CardImageColorProperty, value); }
        }

        public static readonly BindableProperty CardImageGlyphProperty =
            BindableProperty.Create("CardImageGlyph", typeof(string), typeof(CardView), null);

        public string CardImageGlyph
        {
            get { return (string)GetValue(CardImageGlyphProperty); }
            set { SetValue(CardImageGlyphProperty, value); }
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text", typeof(string), typeof(CardView), null);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create("TextColor", typeof(Color), typeof(CardView), null);

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public CardView()
        {
            InitializeComponent();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandProperty, new Binding("TapCommand", source: this));
            cardPage.GestureRecognizers.Add(tapGestureRecognizer);

            imageCard.SetBinding(FontImageSource.ColorProperty, new Binding("CardImageColor", source: this));
            imageCard.SetBinding(FontImageSource.GlyphProperty, new Binding("CardImageGlyph", source: this));

            labelCard.SetBinding(Label.TextProperty, new Binding("Text", source: this));
            labelCard.SetBinding(Label.TextColorProperty, new Binding("TextColor", source: this));
        }
    }
}