﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Avalon.Controls
{
    /// <summary>
    /// Interaction logic for the TriggerList editor.
    /// </summary>
    public partial class TriggerList : UserControl
    {
        /// <summary>
        /// Provided because of binding.
        /// </summary>
        public bool TriggersEnabled
        {
            get => App.Settings.ProfileSettings.TriggersEnabled;
            set => App.Settings.ProfileSettings.TriggersEnabled = value;
        }

        /// <summary>
        /// Timer that sets the delay on your filtering TextBox.
        /// </summary>
        DispatcherTimer _typingTimer;

        public TriggerList()
        {
            InitializeComponent();
            _typingTimer = new DispatcherTimer();
            _typingTimer.Tick += this._typingTimer_Tick;
            DataContext = this;
        }

        /// <summary>
        /// This will effectively load the list data into the DataGrid the first time it's shown to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Load the Trigger list the first time that it's requested.
            if (DataList.ItemsSource == null)
            {
                var lcv = new ListCollectionView(App.Settings.ProfileSettings.TriggerList)
                {
                    Filter = Filter
                };

                DataList.ItemsSource = lcv;
            }

            // Manually setup the bindings.  I couldn't get it to work in the Xaml because the AppSettings gets replaced
            // after this control is loaded.
            var binding = new Binding
            {
                Source = App.Settings.ProfileSettings,
                Path = new PropertyPath("TriggersEnabled"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            BindingOperations.SetBinding(CheckBoxTriggersEnabled, CheckBox.IsCheckedProperty, binding);
        }

        /// <summary>
        /// Reloads the DataList's ItemSource if it's changed.
        /// </summary>
        public void Reload()
        {
            DataList.ItemsSource = null;
            DataList.ItemsSource = App.Settings.ProfileSettings.TriggerList;
            DataList.Items.Refresh();

            // Manually setup the bindings.  I couldn't get it to work in the Xaml because the AppSettings gets replaced
            // after this control is loaded.
            var binding = new Binding
            {
                Source = App.Settings.ProfileSettings,
                Path = new PropertyPath("TriggersEnabled"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            BindingOperations.ClearAllBindings(CheckBoxTriggersEnabled);
            BindingOperations.SetBinding(CheckBoxTriggersEnabled, CheckBox.IsCheckedProperty, binding);
        }

        /// <summary>
        /// The typing delay timer's tick that will refresh the filter after 300ms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>
        /// It's important to stop this timer when this fires so that it doesn't continue
        /// to fire until it's needed again.
        /// </remarks>
        private void _typingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            ((ListCollectionView)DataList.ItemsSource).Refresh();
        }

        /// <summary>
        /// The actual filter that's used to filter down the DataGrid.
        /// </summary>
        /// <param name="item"></param>
        private bool Filter(object item)
        {
            if (string.IsNullOrWhiteSpace(TextFilter.Text))
            {
                return true;
            }

            var trigger = (Common.Triggers.Trigger)item;
            
            return trigger.Pattern.Contains(TextFilter.Text)
                   || trigger.Command.Contains(TextFilter.Text)
                   || trigger.Character.Contains(TextFilter.Text)
                   || trigger.Group.Contains(TextFilter.Text);
        }

        /// <summary>
        /// The filter's text changed event that will setup the delay timer and effective callback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _typingTimer.Stop();
            _typingTimer.Interval = TimeSpan.FromMilliseconds(300);
            _typingTimer.Start();
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            // Get the Trigger from the current line.
            var trigger = ((FrameworkElement)sender).DataContext as Common.Triggers.Trigger;

            // Hmm, no Trigger.. gracefully exit.
            if (trigger == null)
            {
                return;
            }

            // Set the initial text for the editor.
            var win = new StringEditor
            {
                Text = trigger.Command
            };

            win.EditorMode = StringEditor.EditorType.Text;

            // Show the Lua dialog.
            var result = win.ShowDialog();

            // Startup position of the dialog should be in the center of the parent window.  The
            // owner has to be set for this to work.
            win.Owner = App.MainWindow;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // If the result
            if (result != null && result.Value)
            {
                trigger.Command = win.Text;
            }
        }

    }
}
