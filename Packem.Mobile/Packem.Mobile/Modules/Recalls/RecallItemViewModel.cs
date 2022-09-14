using Newtonsoft.Json;
using Packem.Domain.Models;
using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.Data;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Models;
using Packem.Mobile.ViewModels.Recalls;
using Syncfusion.SfDataGrid.XForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Recalls
{
    [QueryProperty("RecallId", "id")]
    public class RecallItemViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IRecallService _recallService;
        private readonly IUnauthorizedService _unauthorizedService;
        private static readonly WeakEventManager<RecallQueueAndDetailViewModel> _selectedItemEventManager =
            new WeakEventManager<RecallQueueAndDetailViewModel>();
        private static readonly WeakEventManager<IEnumerable<RecallQueueLookupGetModel>> _reloadViewEventManager =
            new WeakEventManager<IEnumerable<RecallQueueLookupGetModel>>();

        private int recallId;
        private RecallQueueLookupViewModel recall;
        private bool hasRecord;
        private bool startVisible;
        private bool pauseVisible;

        #endregion

        #region "Properties"

        public int RecallId
        {
            get => recallId;
            set => SetProperty(ref recallId, value);

        }

        public RecallQueueLookupViewModel Recall
        {
            get => recall;
            set => SetProperty(ref recall, value);

        }

        public ObservableRangeCollection<RecallDetailViewModel> RecallDetails { get; }

        public bool HasRecord
        {
            get => hasRecord;
            set => SetProperty(ref hasRecord, value);
        }

        public bool StartVisible
        {
            get => startVisible;
            set => SetProperty(ref startVisible, value);
        }

        public bool PauseVisible
        {
            get => pauseVisible;
            set => SetProperty(ref pauseVisible, value);
        }

        #endregion

        #region "Commands"

        public ICommand AppearingCommand
            => new AsyncCommand(Appearing,
                onException: x => Console.WriteLine(x));

        public ICommand GridTapCommand
            => new AsyncCommand<GridSelectionChangedEventArgs>(GridTap,
                onException: x => Console.WriteLine(x),
                canExecute: x => IsNotBusy);

        public ICommand StartCommand
            => new AsyncCommand(Start,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand PauseCommand
            => new AsyncCommand(Pause,
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

        private void UpdatePickingStatus()
        {
            if (Recall.Status == Domain.Common.Enums.RecallStatusEnum.Pending
                    || Recall.Status == Domain.Common.Enums.RecallStatusEnum.Pause)
            {
                StartVisible = true;
                PauseVisible = false;
            }
            else
            {
                StartVisible = false;
                PauseVisible = true;
            }
        }

        private void OnSelectedItem(RecallQueueLookupViewModel e)
        {
            try
            {
                IsBusy = true;

                Recall = e;
                UpdatePickingStatus();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private IEnumerable<RecallDetailViewModel> DtoToVm(IEnumerable<RecallDetailGetModel> dto)
        {
            var vm = new List<RecallDetailViewModel>();

            foreach (var x in dto)
            {
                var r = new RecallDetailViewModel();
                r.ZoneId = x.ZoneId;
                r.ZoneName = x.ZoneName;
                r.BinId = x.BinId;
                r.BinName = x.BinName;
                r.Qty = x.Qty;
                r.Received = x.Received;
                r.Remaining = x.Remaining;
                
                vm.Add(r);
            }

            return vm;
        }

        private async Task GridTap(GridSelectionChangedEventArgs e)
        {
            try
            {
                IsBusy = true;

                var selectedItem = e.AddedItems[0] as RecallDetailViewModel;

                if (selectedItem != null)
                {
                    if (Recall.Status == Domain.Common.Enums.RecallStatusEnum.Picking)
                    {
                        await _navigationService.PushAsync<RecallPickingViewModel>($"id={RecallId}&bid={selectedItem.BinId}");
                        _selectedItemEventManager.RaiseEvent(new RecallQueueAndDetailViewModel
                        {
                            Recall = Recall,
                            RecallDetail = selectedItem
                        }, nameof(SelectedItem));
                    }
                    else
                    {
                        await _dialogService.DisplayAlert("Warning", "Click START to do recall.", "OK");
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Start()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _recallService
                    .UpdateRecallStatusDeviceAsync(state.DeviceState, new RecallStatusUpdateModel
                    {
                        RecallId = RecallId,
                        Status = Domain.Common.Enums.RecallStatusEnum.Picking
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        Recall.Status = Domain.Common.Enums.RecallStatusEnum.Picking;
                        UpdatePickingStatus();
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        private async Task Pause()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _recallService
                    .UpdateRecallStatusDeviceAsync(state.DeviceState, new RecallStatusUpdateModel
                    {
                        RecallId = RecallId,
                        Status = Domain.Common.Enums.RecallStatusEnum.Pause
                    });

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        Recall.Status = Domain.Common.Enums.RecallStatusEnum.Pause;
                        UpdatePickingStatus();
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }

        public static event Action<RecallQueueAndDetailViewModel> SelectedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        public static event Action<IEnumerable<RecallQueueLookupGetModel>> ReloadView
        {
            add => _reloadViewEventManager.AddEventHandler(value);
            remove => _reloadViewEventManager.RemoveEventHandler(value);
        }

        private void OnReloadView(IEnumerable<RecallDetailGetModel> e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        RecallDetails.Clear();
                        RecallDetails.AddRange(DtoToVm(e));
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnReloadView3(RecallDetailGetModel e)
        {
            try
            {
                IsBusy = true;

                if (e != null)
                {
                    Device.BeginInvokeOnMainThread(() => {
                        var detail = RecallDetails.SingleOrDefault(x => x.BinId == e.BinId);
                        detail.Qty = e.Qty;
                        detail.Received = e.Received;
                        detail.Remaining = e.Remaining;

                        var index = RecallDetails.IndexOf(detail);
                        if (index != -1)
                        {
                            RecallDetails.RemoveAt(index);
                            RecallDetails.Insert(index, detail);
                        }
                    });
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        public RecallItemViewModel(INavigationService navigationService,
            IDialogService dialogService,
            IRecallService recallService,
            IUnauthorizedService unauthorizedService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _recallService = recallService;
            _unauthorizedService = unauthorizedService;

            RecallDetails = new ObservableRangeCollection<RecallDetailViewModel>();
            BindingBase.EnableCollectionSynchronization(RecallDetails, null, ObservableCollectionCallback);

            RecallQueueViewModel.SelectedItem += OnSelectedItem;
            RecallPickingViewModel.ReloadView += OnReloadView;
            RecallPickingViewModel.ReloadView3 += OnReloadView3;
        }

        void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            // `lock` ensures that only one thread access the collection at a time
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }

        public override async Task InitializeAsync()
        {
            try
            {
                IsBusy = true;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndRunning();

                var stateJson = Preferences.Get(Constants.MOBILE_STATE, null);
                var state = JsonConvert.DeserializeObject<MobileState>(stateJson);
                var result = await _recallService
                    .GetRecallDetailDeviceAsync(state.DeviceState, RecallId);

                RecallDetails.CollectionChanged += (sender, args) =>
                {
                    if (RecallDetails.Count() > 0)
                        HasRecord = true;
                    else
                        HasRecord = false;
                };

                await _unauthorizedService
                    .UnauthorizedWorkflowIfNotAuthenticated(result, () =>
                    {
                        RecallDetails.Clear();
                        RecallDetails.AddRange(DtoToVm(result.Response));
                    });
            }
            finally
            {
                IsBusy = false;
                ActivityIndicatorViewModel
                    = Controls.ActivityIndicatorViewModel.CreateActivityIndicatorViewModelDefaultAndNotRunning();
            }
        }
    }
}