// -----------------------------------------------------------------------
// <copyright file="BindableBase.cs" company="Niklas Karl">
// Copyright (c) Niklas Karl. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KitHub
{
    public class BindableBase : INotifyPropertyChanged
    {
        public BindableBase()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }

            return false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
