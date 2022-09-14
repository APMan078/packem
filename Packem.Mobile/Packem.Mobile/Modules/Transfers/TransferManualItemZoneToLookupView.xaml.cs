﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Transfers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferManualItemZoneToLookupView : ContentPage
    {
        public TransferManualItemZoneToLookupView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<TransferManualItemZoneToLookupViewModel>();
        }
    }
}