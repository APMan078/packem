using Autofac;
using Packem.Mobile.Common.MarkupExtensions;
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
    public partial class ActivityIndicatorView : AbsoluteLayout
    {
        public static readonly BindableProperty ActivityIndicatorViewModelProperty =
            BindableProperty.Create("ActivityIndicatorViewModel", typeof(ActivityIndicatorViewModel), typeof(ActivityIndicatorView), null);

        public ActivityIndicatorViewModel ActivityIndicatorViewModel
        {
            get { return (ActivityIndicatorViewModel)GetValue(ActivityIndicatorViewModelProperty); }
            set { SetValue(ActivityIndicatorViewModelProperty, value); }
        }

        public ActivityIndicatorView()
        {
            InitializeComponent();

            activityIndicatorViewPage.SetBinding(AbsoluteLayout.IsVisibleProperty,
                new Binding("ActivityIndicatorViewModel", source: this,
                converter: new ActivityIndicatorViewModelToIsVisibleConverter()));

            innerContainer.SetBinding(StackLayout.OpacityProperty,
                new Binding("ActivityIndicatorViewModel", source: this,
                converter: new ActivityIndicatorViewModelToOpacityConverter()));

            labelMessage.SetBinding(Label.IsVisibleProperty,
                new Binding("ActivityIndicatorViewModel", source: this,
                converter: new ActivityIndicatorViewModelToMessageIsVisibleConverter()));
            labelMessage.SetBinding(Label.TextProperty,
                new Binding("ActivityIndicatorViewModel", source: this,
                converter: new ActivityIndicatorViewModelToMessageConverter()));

            //BindingContext = App.Container.Resolve<ActivityIndicatorViewModel>();
        }
    }
}