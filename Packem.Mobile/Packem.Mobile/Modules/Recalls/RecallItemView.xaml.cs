using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Recalls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RecallItemView : ContentPage
	{
		public RecallItemView ()
		{
			InitializeComponent ();
			BindingContext = App.Container.Resolve<RecallItemViewModel>();
		}
	}
}