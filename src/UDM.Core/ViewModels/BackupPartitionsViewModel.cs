﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UDM.Model;
using UDM.Model.Commands;

namespace UDM.Core.ViewModels
{
    public class BackupPartitionsViewModel(Action closeWindow) : BaseViewModel
    {
        public ObservableCollection<string> BeforeSelectPartitions { get; set; } = new(MainModel.ModelDeviceManager.ActiveDevice.Partitions.Keys.ToList());
        public ObservableCollection<string> AfterSelectPartitions { get; set; } = new();
        public string? SelectedPartitions;

        public ICommand ApplyCommand { get; } = new DelegateCommand(ApplyFunction, DelegateCommand.DefaultCanExecute, closeWindow);

        public void SelectPartition(string? partition)
        {
            BeforeSelectPartitions.Remove(partition!);
            AfterSelectPartitions.Add(partition!);
            OnPropertyChanged(nameof(AfterSelectPartitions));
        }

        static void ApplyFunction(object param)
        {
            if (param is ObservableCollection<string> partitions)
            {
                var scriptCode = "";
                string savePath = MainModel.GetFolderPath?.Invoke() ?? MainModel.Cwd + @"\images";
                if(partitions.Count == 0)
                {
                    MainModel.UiMsgDialog?.Invoke("Warning", "No partitions selected! ");
                    return;
                }
                Dictionary<string, string> blocks = new();    
                foreach (var partition in partitions) { 
                    foreach(var pair in MainModel.ModelDeviceManager.ActiveDevice.Partitions)
                    {
                        if (pair.Key == partition) scriptCode += $"adb_backup {pair.Value.Replace("\n", "").Replace("\r", "")}" +
                                $" {savePath}\\{pair.Value.Split("/")[^1].Replace("\n", "").Replace("\r", "")}_{pair.Key.Replace("\n", "").Replace("\r", "")}.img\r\n";
                    }
                }

                MainModel.CurrentScriptCode = scriptCode;
                MainModel.ModelExecuteCode?.Invoke();
                MainModel.UiMsgDialog?.Invoke("Backup", "Backup saved to " + savePath);
            }
            else return;
        }
    }
}
