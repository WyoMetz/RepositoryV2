using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Repository;

namespace IDRAdministration.ViewModels
{
    class UserAccountsViewModel : ObservableBase
    {
        public UserAccountsViewModel()
        {
            BuildLists();
        }

        IList<User> UserAccounts = new List<User>();

        public async void BuildLists()
        {
            UserAccounts = await new User().GetUsers();
            PinCodes = await new PinCode().GetPinCodes();
            Users = UserAccounts;
        }

        private IList<User> users;
        public IList<User> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        private User selectedUser;
        public User SelectedUser
        {
            get
            {
                return selectedUser;
            }
            set
            {
                selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public ICommand ResetUser
        {
            get { return new RelayCommand(execute => UserReset()); }
        }

        public void UserReset()
        {
            SelectedUser.PrimaryHue = "blue";
            SelectedUser.AccentColor = "red";
            SelectedUser.UsesDarkTheme = true;
            SelectedUser.BackgroundPath = @".\Resources\Camo2.jpg";
            new Database().UpdateEntry(SelectedUser);
            Message = new SnackbarMessageQueue();
            Message.Enqueue($"Account for {SelectedUser.UserName} has been reset");
            SelectedUser = null;
        }

        private string search;
        public string Search
        {
            get
            {
                return search;
            }
            set
            {
                search = value;
                SearchUser();
                OnPropertyChanged("Search");
            }
        }

        private void SearchUser()
        {
            List<User> tempUsers = UserAccounts.ToList();
            if (!string.IsNullOrEmpty(search))
            {
                Users = tempUsers.Where(x => x.ToString().Contains(search.ToUpper())).ToList();
            }
        }

        private SnackbarMessageQueue message;
        public SnackbarMessageQueue Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }

        private PinCode selectedPin;
        public PinCode SelectedPin
        {
            get
            {
                return selectedPin;
            }
            set
            {
                selectedPin = value;
                OnPropertyChanged("SelectedPin");
            }
        }

        private IList<PinCode> pinCodes;
        public IList<PinCode> PinCodes
        {
            get
            {
                return pinCodes;
            }
            set
            {
                pinCodes = value;
                OnPropertyChanged("PinCodes");
            }
        }

        public ICommand UpdatePin
        {
            get
            {
                return new RelayCommand(execute => SelectedPin.UpdatePin());
            }
        }
    }
}
