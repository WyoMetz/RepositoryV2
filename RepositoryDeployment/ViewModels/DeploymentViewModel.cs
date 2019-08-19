using RepositoryDeployment.Views;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace RepositoryDeployment.ViewModels
{
    public class DeploymentViewModel : ObservableBase
    {
        NavigationService Navigation;
        public DeploymentViewModel()
        {

        }

        #region Commands

        private int StepCount = 0;

        public ICommand MoveForward
        {
            get { return new RelayCommand(execute => NavigateForward()); }
        }

        private void NavigateForward()
        {
            Navigation = NavigationHelper.Navigation;
            StepCount++;
            if (StepCount == 1)
            {
                Navigation.Navigate(new Page1());
            }
            if (StepCount == 2)
            {
                Navigation.Navigate(new Page2());
            }
            if (StepCount == 3)
            {
                Navigation.Navigate(new Page3());
            }
        }

        public ICommand MoveBack
        {
            get { return new RelayCommand(execute => NavigateBackward()); }
        }

        private void NavigateBackward()
        {
            Navigation = NavigationHelper.Navigation;
            if (UpdateChecked == true)
            {
                ExecuteNavigation(new BeginUpdate());
            }
            else
            {
                StepCount--;
                AdvancedChecked = false;
                if (StepCount == 1)
                {
                    Navigation.Navigate(new Page1());
                }
                if (StepCount == 2)
                {
                    Navigation.Navigate(new Page2());
                }
                if (StepCount == 3)
                {
                    Navigation.Navigate(new Page3());
                }
            }
        }

        public ICommand MovePage1
        {
            get { return new RelayCommand(execute => ExecutePage1()); }
        }

        private void ExecutePage1()
        {
            if (UpdateChecked == true)
            {
                ExecuteNavigation(new BeginUpdate());
            }
            else
            {
                StepCount++;
                ExecuteNavigation(new Page1());
            }
        }

        public ICommand MovePage2
        {
            get { return new RelayCommand(execute => ExecutePage2()); }
        }

        private void ExecutePage2()
        {
            StepCount++;
            ExecuteNavigation(new Page2());
        }

        public ICommand MovePage3
        {
            get { return new RelayCommand(execute => ExecutePage3(), canExecute => CanExecutePage3()); }
        }

        private void ExecutePage3()
        {
            StepCount++;
            DeploymentLocation = string.Concat(BaseLocation, @"\IDR");
            DatabaseLocation = string.Concat(DeploymentLocation, @"\Repository");
            DocumentLocation = string.Concat(DeploymentLocation, @"\Repository");
            ApplicationLocation = string.Concat(DeploymentLocation, @"\Applications");
            if (UpdateChecked == true)
            {
                if(AdvancedChecked == true)
                {
                    ExecuteNavigation(new UpdateAdvanced());
                }
                else
                {
                    ExecuteNavigation(new UpdateReady());
                }
            }
            else
            {
                ExecuteNavigation(new Page3());
            }
            ConfirmChecked = false;
        }

        private bool CanExecutePage3()
        {
            if (ConfirmChecked == true && !string.IsNullOrEmpty(BaseLocation))
            {
                return true;
            }
            return false;
        }

        public ICommand ContinueDeployment
        {
            get { return new RelayCommand(execute => ExecuteContinue()); }
        }

        private void ExecuteContinue()
        {
            if (AdvancedChecked == true)
            {
                ExecuteNavigation(new Advanced());
            }
            else
            {
                ExecuteNavigation(new Default());
            }
        }

        public ICommand Deploy
        {
            get { return new RelayCommand(execute => ExecuteDeployment(), canExecute => ConfirmChecked); }
        }

        private void ExecuteDeployment()
        {
            ExecuteNavigation(new Deployment());
            ConfirmChecked = false;
            Task.Run(() => RunDeployment());
        }

        public ICommand Finished
        {
            get { return new RelayCommand(execute => ExecuteFinish(), canExecute => ConfirmChecked); }
        }

        private void ExecuteFinish()
        {
            Application.Current.MainWindow.Close();
        }

        public ICommand UpdateNext
        {
            get { return new RelayCommand(execute => ExecuteUpdateNext()); }
        }

        private void ExecuteUpdateNext()
        {
            ExecuteNavigation(new UpdateDefault());
        }

        public ICommand RunUpdate
        {
            get { return new RelayCommand(execute => ExecuteRunUpdate(), canExecute => ConfirmChecked); }
        }

        private void ExecuteRunUpdate()
        {
            ExecuteNavigation(new Update());
            ConfirmChecked = false;
            Task.Run(() => RunUpdates());
        }

        private void ExecuteNavigation(object rootPage)
        {
            Navigation = NavigationHelper.Navigation;
            Navigation.Navigate(rootPage);
        }

        #endregion

        #region Objects

        private string baseLocation;
        public string BaseLocation
        {
            get
            {
                return baseLocation;
            }
            set
            {
                baseLocation = value;
                OnPropertyChanged("BaseLocation");
            }
        }

        private string deploymentLocation;
        public string DeploymentLocation
        {
            get
            {
                return deploymentLocation;
            }
            set
            {
                deploymentLocation = value;
                OnPropertyChanged("DeploymentLocation");
            }
        }

        private bool confirmChecked;
        public bool ConfirmChecked
        {
            get
            {
                return confirmChecked;
            }
            set
            {
                confirmChecked = value;
                OnPropertyChanged("ConfirmChecked");
            }
        }

        private bool existingChecked;
        public bool ExistingChecked
        {
            get
            {
                return existingChecked;
            }
            set
            {
                existingChecked = value;
                OnPropertyChanged("ExistingChecked");
            }
        }

        private bool advancedChecked;
        public bool AdvancedChecked
        {
            get
            {
                return advancedChecked;
            }
            set
            {
                advancedChecked = value;
                OnPropertyChanged("AdvancedChecked");
            }
        }

        private int deploymentProgress;
        public int DeploymentProgress
        {
            get
            {
                return deploymentProgress;
            }
            set
            {
                deploymentProgress = value;
                OnPropertyChanged("DeploymentProgress");
            }
        }

        private string deploymentMessage;
        public string DeploymentMessage
        {
            get
            {
                return deploymentMessage;
            }
            set
            {
                deploymentMessage = value;
                OnPropertyChanged("DeploymentMessage");
            }
        }

        private string databaseLocation;
        public string DatabaseLocation
        {
            get
            {
                return databaseLocation;
            }
            set
            {
                if (value.StartsWith(@"\"))
                {
                    databaseLocation = value;
                    OnPropertyChanged("DatabaseLocation");
                }
                else
                {
                    new HelperWindow().Show();
                    databaseLocation = value;
                    OnPropertyChanged("DatabaseLocation");
                }
            }
        }

        private string documentLocation;
        public string DocumentLocation
        {
            get
            {
                return documentLocation;
            }
            set
            {
                documentLocation = value;
                OnPropertyChanged("DocumentLocation");
            }
        }

        private string applicationLocation;
        public string ApplicationLocation
        {
            get
            {
                return applicationLocation;
            }
            set
            {
                applicationLocation = value;
                OnPropertyChanged("ApplicationLocation");
            }
        }

        private bool updateChecked;
        public bool UpdateChecked
        {
            get
            {
                return updateChecked;
            }
            set
            {
                updateChecked = value;
                OnPropertyChanged("UpdateChecked");
            }
        }

        private bool exisitingDatabase;
        public bool ExistingDatabase
        {
            get
            {
                return exisitingDatabase;
            }
            set
            {
                exisitingDatabase = value;
                OnPropertyChanged("ExistingDatabase");
            }
        }

        #endregion

        #region Logic

        private string[] Messages = new string[]
        {
            "Configuring Repository", "Updating Location Files", "Preparing Applications", "Deploying Applications", "Setting Up Shortcuts"
        };

        private string[] UpdateMessages = new string[]
        {
            "Configuring Repository", "Updating Location Files", "Preparing Applications", "Updating Applications", "Setting Up Shortcuts"
        };

        private async Task RunDeployment()
        {
            DeploymentSystem deployment = new DeploymentSystem();
            deployment.DatabaseFolder = DatabaseLocation;
            deployment.FileRepository = DocumentLocation;
            deployment.ApplicationFolder = ApplicationLocation;
            deployment.Update = UpdateChecked;
            Task.Run(() => deployment.Deploy());
            DeploymentProgress = 0;
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(300);
                DeploymentProgress++;
                DeploymentMessage = Messages[(i / 20)];
            }
            DeploymentMessage = "Complete!";
            ConfirmChecked = true;
        }

        private async Task RunUpdates()
        {
            DeploymentSystem deployment = new DeploymentSystem();
            deployment.DatabaseFolder = DatabaseLocation;
            deployment.FileRepository = DocumentLocation;
            deployment.ApplicationFolder = ApplicationLocation;
            deployment.Update = UpdateChecked;
            DeploymentProgress = 0;
            Task.Run(() => deployment.Deploy());
            for (int i = 0; i < 100; i++)
            {
                await Task.Delay(300);
                DeploymentProgress++;
                DeploymentMessage = UpdateMessages[(i / 20)];
            }
            DeploymentMessage = "Complete!";
            ConfirmChecked = true;
        }

        #endregion
    }
}
