using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LabelForNoRecordFound : Label
    {
        public static readonly BindableProperty NoRecordFoundTextProperty =
            BindableProperty.Create("NoRecordFoundText", typeof(string), typeof(LabelForNoRecordFound), null);

        public string NoRecordFoundText
        {
            get { return (string)GetValue(NoRecordFoundTextProperty); }
            set { SetValue(NoRecordFoundTextProperty, value); }
        }

        public LabelForNoRecordFound()
        {
            InitializeComponent();

            labelNoRecordFound.SetBinding(Label.TextProperty, new Binding("NoRecordFoundText", source: this));
        }
    }
}