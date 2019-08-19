using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace RepositoryDeployment
{
    public static class NavigationHelper
    {
        private static NavigationService navigation;
        public static NavigationService Navigation
        {
            get
            {
                return navigation;
            }
            set
            {
                navigation = value;
            }
        }
    }
}
