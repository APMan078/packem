﻿using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Packem.Mobile.Modules.Picking
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PickQueueScanView : ContentPage
    {
        public PickQueueScanView()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<PickQueueScanViewModel>();
        }
    }
}