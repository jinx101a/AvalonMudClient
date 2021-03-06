﻿using Avalon.Common.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Avalon.Controls
{
    /// <summary>
    /// Interaction logic for the MacroList editor.
    /// </summary>
    public partial class MacroList : UserControl
    {

        /// <summary>
        /// Timer that sets the delay on your filtering TextBox.
        /// </summary>
        DispatcherTimer _typingTimer;

        public MacroList()
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
            // Load the macro list the first time that it's requested.
            if (DataList.ItemsSource == null)
            {
                var lcv = new ListCollectionView(App.Settings.ProfileSettings.MacroList)
                {
                    Filter = Filter
                };

                DataList.ItemsSource = lcv;
            }
        }

        /// <summary>
        /// Reloads the DataList's ItemSource if it's changed.
        /// </summary>
        public void Reload()
        {
            DataList.ItemsSource = null;
            DataList.ItemsSource = App.Settings.ProfileSettings.MacroList;
            DataList.Items.Refresh();
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

            var macro = (Macro)item;

            return macro.Command.Contains(TextFilter.Text);
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
            // Get the macro from the current line.
            var macro = ((FrameworkElement)sender).DataContext as Macro;

            // Hmm, no macro.. gracefully exit.
            if (macro == null)
            {
                return;
            }

            // Set the initial text for the editor.
            var win = new StringEditor
            {
                Text = macro.Command
            };

            // Set this to be a text editor.
            win.EditorMode = StringEditor.EditorType.Text;

            // Startup position of the dialog should be in the center of the parent window.  The
            // owner has to be set for this to work.
            win.Owner = App.MainWindow;
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            // Show the string dialog
            var result = win.ShowDialog();

            // If the result
            if (result != null && result.Value)
            {
                macro.Command = win.Text;
            }
        }

    }
}
