﻿using System.ComponentModel;

namespace Avalon.Common.Models
{
    public class Direction : INotifyPropertyChanged
    {
        public Direction()
        {

        }

        public Direction(string name, string speedwalk)
        {
            this.Name = name;
            this.Speedwalk = speedwalk;
        }

        public Direction(string name, string speedwalk, string startingRoom)
        {
            this.Name = name;
            this.Speedwalk = speedwalk;
            this.StartingRoom = startingRoom;
        }

        public string Name { get; set; } = "";

        private string _speedwalk = "";

        public string Speedwalk
        {
            get
            {
                return _speedwalk;
            }
            set
            {
                _speedwalk = value;
                OnPropertyChanged("Speedwalk");
            }
        }

        public string StartingRoom { get; set; } = "";

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
