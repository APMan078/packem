using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packem.Mobile.Modules.Recalls
{
    public class RecallPickingBinScanViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;

        #endregion

        #region "Properties"
        #endregion

        #region "Commands"
        #endregion

        #region "Functions"
        #endregion

        public RecallPickingBinScanViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}