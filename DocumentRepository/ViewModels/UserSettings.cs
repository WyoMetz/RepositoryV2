using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Repository;

namespace DocumentRepository.ViewModels
{
    public class UserSettings : ObservableBase
    {
        public User User { get; set; }

        private string primaryHue;
        private BitmapImage backgroundImage;
        private string accentColor;
        private bool usesDarkTheme;

        IList<int> arucs = new List<int>();
        IList<int> years = new List<int>();

        public UserSettings()
        {
            SetUpUser();
            PrimaryHue = User.PrimaryHue;
            AccentColor = User.AccentColor;
            BackgroundPath = User.BackgroundPath;
            UsesDarkTheme = User.UsesDarkTheme;
            AppSettings.Aruc = User.Aruc;
            SetBackground();
            CurrentAruc = User.Aruc;
            CurrentYear = AppSettings.Year;
            CurrentArucs = arucs;
            CurrentYears = years;
            IsPreparer = Visibility.Collapsed;
            IsApprover = Visibility.Collapsed;
            IsVerifier = Visibility.Collapsed;
            OpenBox = false;
        }

        private async void SetUpUser()
        {
            arucs = await new Database().GetArucs();
            years = await new Database().GetYears();
            User = await new User().GetUserSettings();
            PinCodes = await new PinCode().GetPinCodes();

            if (string.IsNullOrEmpty(User.BackgroundPath))
            {
                User.PrimaryHue = "blue";
                User.AccentColor = "red";
                User.UsesDarkTheme = true;
                User.BackgroundPath = @".\Resources\Camo2.jpg";
                User.CurrentFilePath = Path.GetFullPath(User.BackgroundPath);
                User.Aruc = arucs.FirstOrDefault();
                new Database().CreateEntry(User);
            }
            if(User.Aruc == 0)
            {
                User.Aruc = arucs.FirstOrDefault();
                new Database().UpdateEntry(User);
            }
        }

        private IList<string> primaryValues = new List<string>()
        {
            "red", "blue", "yellow", "amber", "deeporange", "lightblue", "teal", "cyan", "pink", "green", "deeppurple", "indigo", "lightgreen", "lime", "red", "orange", "purple", "bluegrey", "grey", "brown"
        };

        private IList<string> accentValues = new List<string>()
        {
            "blue", "yellow", "amber", "deeporange", "lightblue", "teal", "cyan", "pink", "green", "deeppurple", "indigo", "lightgreen", "lime", "red", "orange", "purple"
        };

        public string PrimaryHue
        {
            get
            {
                return primaryHue;
            }
            set
            {
                primaryHue = value;
                setPalette();
                OnPropertyChanged("PrimaryHue");
            }
        }

        public IList<string> PrimaryValues
        {
            get
            {
                return primaryValues;
            }
            set
            {
                primaryValues = value;
                OnPropertyChanged("PrimaryValues");
            }
        }

        public IList<string> AccentValues
        {
            get
            {
                return accentValues;
            }
            set
            {
                accentValues = value;
                OnPropertyChanged("AccentValues");
            }
        }

        public string AccentColor
        {
            get
            {
                return accentColor;
            }
            set
            {
                accentColor = value;
                setSecondaryPalette();
                OnPropertyChanged("AccentColor");
            }
        }

        public BitmapImage BackgroundImage
        {
            get
            {
                return backgroundImage;
            }
            set
            {
                backgroundImage = value;
                setPalette();
                OnPropertyChanged("BackgroundImage");
            }
        }

        public bool UsesDarkTheme
        {
            get
            {
                return usesDarkTheme;
            }
            set
            {
                usesDarkTheme = value;
                setTheme();
                OnPropertyChanged("UsesDarkTheme");
            }
        }

        private string backgroundPath;
        public string BackgroundPath
        {
            get
            {
                return backgroundPath;
            }
            set
            {
                backgroundPath = value;
                SetBackground();
                OnPropertyChanged("BackgroundPath");
            }
        }

        private IList<int> currentYears;
        public IList<int> CurrentYears
        {
            get
            {
                return currentYears;
            }
            set
            {
                currentYears = value;
                OnPropertyChanged("CurrentYears");
            }
        }

        private IList<int> currentArucs;
        public IList<int> CurrentArucs
        {
            get
            {
                return currentArucs;
            }
            set
            {
                currentArucs = value;
                OnPropertyChanged("CurrentArucs");
            }
        }

        private int currentYear;
        public int CurrentYear
        {
            get
            {
                return currentYear;
            }
            set
            {
                currentYear = value;
                AppSettings.Year = value;
                OnPropertyChanged("CurrentYear");
            }
        }

        private int currentAruc;
        public int CurrentAruc
        {
            get
            {
                return currentAruc;
            }
            set
            {
                currentAruc = value;
                AppSettings.Aruc = value;
                OnPropertyChanged("CurrentAruc");
            }
        }

        PaletteHelper helper = new PaletteHelper();

        private void setPalette()
        {
            Swatch Primary = new SwatchesProvider().Swatches.Single(o => o.Name == PrimaryHue);
            helper.ReplacePrimaryColor(Primary);
            User.PrimaryHue = PrimaryHue;
        }

        private void setSecondaryPalette()
        {
            Swatch Accent = new SwatchesProvider().Swatches.Single(o => o.Name == AccentColor);
            helper.ReplaceAccentColor(Accent);
            User.AccentColor = AccentColor;
        }

        private void setTheme()
        {
            helper.SetLightDark(UsesDarkTheme);
            User.UsesDarkTheme = UsesDarkTheme;
        }

        private void SetBackground()
        {
            BitmapImage image = new BitmapImage(new Uri(Path.GetFullPath(BackgroundPath)));
            BackgroundImage = image;
        }

        public ICommand GetImage
        {
            get { return new RelayCommand(execute => ExecuteImageSearch()); }
        }

        public string CurrentFilePath { get; set; }

        public void ExecuteImageSearch()
        {
            BackgroundPath = new FileOperation().ChooseFile();
            CurrentFilePath = BackgroundPath;
            User.BackgroundPath = BackgroundPath;
            User.CurrentFilePath = BackgroundPath;
        }

        public ICommand SaveSettings
        {
            get { return new RelayCommand(execute => SaveUserSettings()); }
        }

        private async void SaveUserSettings()
        {
            if(CurrentFilePath != null)
            {
                User.BackgroundPath = await new FileOperation().CopyFile(User);
            }
            new Database().UpdateEntry(User);
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

        private Visibility isVerifier;
        public Visibility IsVerifier
        {
            get
            {
                return isVerifier;
            }
            set
            {
                isVerifier = value;
                OnPropertyChanged("IsVerfier");
            }
        }

        private Visibility isPreparer;
        public Visibility IsPreparer
        {
            get
            {
                return isPreparer;
            }
            set
            {
                isPreparer = value;
                OnPropertyChanged("IsPreparer");
            }
        }

        private Visibility isApprover;
        public Visibility IsApprover
        {
            get
            {
                return isApprover;
            }
            set
            {
                isApprover = value;
                OnPropertyChanged("IsApprover");
            }
        }

        private bool openBox;
        public bool OpenBox
        {
            get
            {
                return openBox;
            }
            set
            {
                openBox = value;
                OnPropertyChanged("OpenBox");
            }
        }

        private int pinCode;
        public int PinCode
        {
            get
            {
                return pinCode;
            }
            set
            {
                pinCode = value;
                CheckAuthority();
                OnPropertyChanged("PinCode");
            }
        }

        private void CheckAuthority()
        {
            IsPreparer = Visibility.Collapsed;
            IsApprover = Visibility.Collapsed;
            IsVerifier = Visibility.Collapsed;
            OpenBox = false;
            if(PinCode == getPin("Verifier"))
            {
                IsVerifier = Visibility.Visible;
                OpenBox = true;
            }
            if(PinCode == getPin("Approver"))
            {
                IsApprover = Visibility.Visible;
                IsVerifier = Visibility.Visible;
                IsPreparer = Visibility.Visible;
                OpenBox = true;
            }
            if(PinCode == getPin("Preparer"))
            {
                IsVerifier = Visibility.Visible;
                IsPreparer = Visibility.Visible;
                OpenBox = true;
            }
        }

        private int getPin(string programName)
        {
            return PinCodes.Where(x => x.ProgramName == programName).First().Pin;
        }
    }
}
