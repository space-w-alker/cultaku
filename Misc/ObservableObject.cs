﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace culTAKU.Misc
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropName));
        }
    }
}
