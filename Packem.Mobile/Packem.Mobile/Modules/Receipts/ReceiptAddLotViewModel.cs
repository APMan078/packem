using Packem.Mobile.Common.Base;
using Packem.Mobile.Common.Interfaces.General;
using Packem.Mobile.Common.Validations;
using Packem.Mobile.Common.Validations.Rules;
using Packem.Mobile.ViewModels.Core;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Packem.Mobile.Modules.Receipts
{
    public class ReceiptAddLotViewModel : BaseViewModel
    {
        #region "Variables"

        private readonly INavigationService _navigationService;
        private static readonly WeakEventManager<LotViewModel> _selectedItemEventManager =
            new WeakEventManager<LotViewModel>();

        private ValidatableObject<string> _lotNo;
        private ValidatableObject<string> _expirationDate;
        private bool _pickerOpen;

        #endregion

        #region "Properties"

        public ValidatableObject<string> LotNo
        {
            get => _lotNo;
            set { SetProperty(ref _lotNo, value); }
        }

        public ValidatableObject<string> ExpirationDate
        {
            get => _expirationDate;
            set { SetProperty(ref _expirationDate, value); }
        }

        public bool PickerOpen
        {
            get => _pickerOpen;
            set { SetProperty(ref _pickerOpen, value); }
        }

        #endregion

        #region "Commands"

        public ICommand LotNoUnfocusedCommand
            => new Command(LotNoUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand ExpirationDateUnfocusedCommand
            => new Command(ExpirationDateUnfocused,
                canExecute: () => IsNotBusy);

        public ICommand SearchExpirationDateCommand
            => new Command(SearchExpirationDate,
                canExecute: () => IsNotBusy);

        public ICommand ChangeExpirationDateCommand
            => new Command<object>(ChangeExpirationDate,
                canExecute: (e) => IsNotBusy);

        public ICommand CancelLotCommand
            => new AsyncCommand(CancelLot,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        public ICommand AddLotCommand
            => new AsyncCommand(AddLot,
                canExecute: x => IsNotBusy,
                onException: x => Console.WriteLine(x));

        #endregion

        #region "Functions"

        private void AddValidations()
        {
            _lotNo = new ValidatableObject<string>();
            _expirationDate = new ValidatableObject<string>();

            _lotNo.Validations.Add(new IsNotNullOrEmptyRule<string>
            {
                ValidationMessage = "Please enter a lot no."
            });

            _expirationDate.Validations.Add(new DateTimeRule<string>
            {
                ValidationMessage = "Please enter expiration date (m/d/y)."
            });
        }

        private bool AreFieldsValid()
        {
            _lotNo.Validate();
            _expirationDate.Validate();

            return _lotNo.IsValid && _expirationDate.IsValid;
        }

        void LotNoUnfocused()
            => _lotNo.Validate();

        void ExpirationDateUnfocused()
            => _expirationDate.Validate();

        private void SearchExpirationDate()
        {
            try
            {
                IsBusy = true;

                PickerOpen = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ChangeExpirationDate(object e)
        {
            try
            {
                IsBusy = true;

                var datePicker = e as Syncfusion.SfPicker.XForms.SelectionChangedEventArgs;
                var date = GetSelectedItems(datePicker.NewValue as ICollection);
                var dateString = date.Remove(date.Length - 1, 1);

                ExpirationDate.Value = dateString;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string GetSelectedItems(ICollection collection)
        {
            string dates = string.Empty;
            int i = 0;
            foreach (var item in collection)
            {
                dates += item;
                dates += "/";
                i++;
            }
            return dates;
        }

        private async Task CancelLot()
        {
            try
            {
                IsBusy = true;

                await _navigationService.PopAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddLot()
        {
            if (!AreFieldsValid())
            {
                return;
            }

            try
            {
                IsBusy = true;

                _selectedItemEventManager.RaiseEvent(new LotViewModel
                {
                    LotId = 0,
                    LotNo = LotNo.Value,
                    ExpirationDate = Convert.ToDateTime(ExpirationDate.Value)
                }, nameof(AddedItem));

                await _navigationService.PopAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public static event Action<LotViewModel> AddedItem
        {
            add => _selectedItemEventManager.AddEventHandler(value);
            remove => _selectedItemEventManager.RemoveEventHandler(value);
        }

        #endregion

        public ReceiptAddLotViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            AddValidations();
        }
    }
}