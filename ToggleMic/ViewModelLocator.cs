using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToggleMic.ViewModels;

namespace ToggleMic
{
    public class ViewModelLocator
    {
        public MainViewModel MainViewModel
        {
            get
            {
                return new MainViewModel();
            }
        }
    }
}
