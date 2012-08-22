using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Skill.Studio.Tools.HToMesh.ViewModel
{
    public class TSWizardViewModel : WizardViewModel<RawFile>
    {        
        protected override RawFile CreateData()
        {
            return new RawFile();
        }

        protected override System.Collections.ObjectModel.ReadOnlyCollection<WizardPageViewModelBase<RawFile>> CreatePages()
        {
            var openVM = new OpenPageViewModel(this.Data);
            var splitVM = new SplitPageViewModel(this.Data);
            var ExportVM = new ExportPageViewModel(this.Data);
            var pages = new List<WizardPageViewModelBase<RawFile>>();

            pages.Add(openVM);
            pages.Add(splitVM);
            pages.Add(ExportVM);

            return new ReadOnlyCollection<WizardPageViewModelBase<RawFile>>(pages);
        }        
    }
}
