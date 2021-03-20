using MvvmCross.ViewModels;
using MvxFotosRadar.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvxFotosRadar.Core
{
    public class App: MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<FotosRadarViewModel>();
        }
    }
}
