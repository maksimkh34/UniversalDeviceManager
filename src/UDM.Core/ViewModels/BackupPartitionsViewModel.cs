using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDM.Model;

namespace UDM.Core.ViewModels
{
    public class BackupPartitionsViewModel : BaseViewModel
    {
        public ObservableCollection<string> BeforeSelectPartitions { get; set; } = new(MainModel.ModelDeviceManager.ActiveDevice.Partitions.Keys.ToList());
        public ObservableCollection<string> AfterSelectPartitions { get; set; } = new();
    }
}
