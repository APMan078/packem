using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Recalls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Recalls
{
    [QueryProperty("RecallId", "id")]
    [QueryProperty("BinId", "bid")]
    public class RecallPickingViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IRecallService _recallService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<IEnumerable<RecallDetailGetModel>> _reloadViewEventManager =
            new WeakEventManager<IEnumerable<RecallDetailGetModel>>();
        private static readonly WeakEventManager<IEnumerable<RecallQueueLookupGetModel>> _reloadViewEventManager2 =
            new WeakEventManager<IEnumerable<RecallQueueLookupGetModel>>();
        private static readonly WeakEventManager<RecallDetailGetModel> _reloadViewEventManager3 =
            new WeakEventManager<RecallDetailGetModel>();

        private int recallId;
        private int binId;
        private RecallQueueAndDetailViewModel recallQueueAndDetail;
        private ValidatableObject<int?> _pickQty;

        #endregion

        #region "Properties"

        public int RecallId
        {
            get => recallId;
            set => SetProperty(ref recallId, value);

        }

        public int BinId
        {
            get => binId;
            set => SetProperty(ref binId, value);

        }

        public RecallQueueAndDetailViewModel RecallQueueAndDetail
        {
            get => recallQueueAndDetail;
            set => SetProperty(ref recallQueueAndDetail, value);

        }

        public ValidatableObject<int?> PickQty
        {
            get => _pickQty;
            set => SetProperty(ref _pickQty, value);

        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand PickQtyUnfocusedCommand
            => new Command(PickQtyUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand UpdateCommand
            => new AsyncCommand(Update,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

        private async Task Appearing()
        {
            if (!Initialized)
            {
                await InitializeAsync();
                Initialized = true;
            }
        }

        private void AddValidations()
        {
            _pickQty = new ValidatableObject<int?>();

            _pickQty.Validations.Add(new IsNotNullRule<int?>
            {
                ValidationMessage = "Please enter pick qty."
            });
        }

        private bool AreFieldsValid()
        {
            _pickQty.Validate();

            return _pickQty.IsValid;
        }

        private void OnSelectedItem(RecallQueueAndDetailViewModel e)
        {
            try
            {
                IsBusy = true;

                RecallQueueAndDetail = e;
            }
            finally
            {
                IsBusy = false;
            }
        }

        void PickQtyUnfocused()
            => _pickQty.Validate();

        private async Task Update()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _recallService
                    .CreateRecallBinDeviceAsync(state.DeviceState, new RecallBinCreateModel
                    {
                        UserId = state.AppState.UserId,
                        RecallId = RecallId,
                        BinId = BinId,
                        Qty = PickQty.Value
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, async () =>
                    {
                        var result2 = await _recallService
                            .GetRecallDetailDeviceAsync(state.DeviceState, RecallId, BinId);

                        if (result2.Success)
                        {
                            //OrderLine.Received = OrderLine.Received + PickQty.Value.Value;
                            //OrderLine.Remaining = OrderLine.Remaining - PickQty.Value.Value;
                            RecallQueueAndDetail.RecallDetail.Qty = result2.Response.Qty;
                            RecallQueueAndDetail.RecallDetail.Received = result2.Response.Received;
                            RecallQueueAndDetail.RecallDetail.Remaining = result2.Response.Remaining;

                            _reloadViewEventManager3.RaiseEvent(result2.Response, nameof(ReloadView3));

                            await _dialogService.DisplayAlert("Recall", "Recall Success.", "OK");

                            PickQty.Value = null;
                        }
                        else
                        {
                            await _dialogService.DisplayAlert("Recall", "Recall Success.", "OK");

                            // if remaining is zero. it will return bad request. because the bin will be inventory deleted
                            if (result2.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            {
                                var result3 = await _recallService
                                    .GetRecallDetailDeviceAsync(state.DeviceState, RecallId);

                                if (result3.Success)
                                {
                                    if (result3.Response.Count() == 0)
                                    {
                                        var result4 = await _recallService
                                            .UpdateRecallStatusDeviceAsync(state.DeviceState, new RecallStatusUpdateModel
                                            {
                                                RecallId = RecallId,
                                                Status = Domain.Common.Enums.RecallStatusEnum.Complete
                                            });

                                        if (result4.Success)
                                        {
                                            RecallQueueAndDetail.Recall.Status = Domain.Common.Enums.RecallStatusEnum.Complete;

                                            var result5 = await _recallService
                                                .GetRecallQueueLookupDeviceAsync(state.DeviceState, state.Facility.CustomerFacilityId, null);

                                            _reloadViewEventManager2.RaiseEvent(result5.Response, nameof(ReloadView2));
                                            await Shell.Current.GoToAsync("../.."); // go to RecallQueueViewModel
                                        }
                                    }
                                    else
                                    {
                                        _reloadViewEventManager.RaiseEvent(result3.Response, nameof(ReloadView));
                                        await _navigationService.PopAsync();
                                    }
                                }
                            }
                        }
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        public static event Action<IEnumerable<RecallDetailGetModel>> ReloadView
        {
            add => _reloadViewEventManager.AddEventHandler(value);
            remove => _reloadViewEventManager.RemoveEventHandler(value);
        }

        public static event Action<IEnumerable<RecallQueueLookupGetModel>> ReloadView2
        {
            add => _reloadViewEventManager2.AddEventHandler(value);
            remove => _reloadViewEventManager2.RemoveEventHandler(value);
        }

        public static event Action<RecallDetailGetModel> ReloadView3
        {
            add => _reloadViewEventManager3.AddEventHandler(value);
            remove => _reloadViewEventManager3.RemoveEventHandler(value);
        }

        #endregion

        public RecallPickingViewModel(INavigationService navigationService,
        IDialogService dialogService,
        IRecallService recallService,
        IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _recallService = recallService;
            _unauthorizedService = unauthorizedService;

            RecallItemViewModel.SelectedItem += OnSelectedItem;

            AddValidations();
        }

        public override Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }

            return base.InitializeAsync();
        }
    }
}