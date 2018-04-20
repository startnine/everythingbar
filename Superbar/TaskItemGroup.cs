using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Start9.Api;
using Start9.Api.Programs;
using Start9.Api.Tools;

namespace Superbar
{
    public class TaskItemGroup : INotifyPropertyChanged
    {
        public enum TabMode
        {
            None,
            Single,
            Double,
            NotRunning
        }

        bool _combineItems = false;

        public bool CombineItems
        {
            get => _combineItems;
            set
            {
                _combineItems = value;
                NotifyPropertyChanged("_combineItems"); //"CombineItems");
            }
        }

        /*public static readonly DependencyProperty CombineItemsProperty =
            DependencyProperty.Register("CombineItems", typeof(bool), typeof(TaskItemGroup), new PropertyMetadata(true));*/

        ObservableCollection<ProgramWindow> _windows = new ObservableCollection<ProgramWindow>();

        public ObservableCollection<ProgramWindow> Windows
        {
            get => _windows;
            set
            {
                _windows = value;
                NotifyPropertyChanged("_windows"); //("Windows");
            }
        }

        /*public static readonly DependencyProperty WindowsProperty =
            DependencyProperty.Register("Windows", typeof(ObservableCollection<ProgramWindow>), typeof(TaskItemGroup), new FrameworkPropertyMetadata(new ObservableCollection<ProgramWindow>(), FrameworkPropertyMetadataOptions.AffectsRender));*/

        /*public int WindowCount
        {
            get
            {
                return (int)GetValue(WindowCountProperty);
            }
            set
            {
                SetValue(WindowCountProperty, value);
            }
        }

        public static readonly DependencyProperty WindowCountProperty =
            DependencyProperty.Register("WindowCount", typeof(int), typeof(TaskItemGroup), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsRender));*/

        TabMode _groupTabMode = TabMode.Double;

        public TabMode GroupTabMode
        {
            get => _groupTabMode;
            set
            {
                _groupTabMode = value;
                NotifyPropertyChanged("_groupTabMode"); //("GroupTabMode");
            }
        }

        ProgramWindow _activeWindowItem = null;

        public ProgramWindow ActiveWindowItem
        {
            get => _activeWindowItem;
            set
            {
                _activeWindowItem = value;
                NotifyPropertyChanged("_activeWindowItem"); //("GroupTabMode");
            }
        }

        bool _isActiveWindow = false;

        public bool IsActiveWindow
        {
            get
            {
                /*bool isActive = false;
                foreach (ProgramWindow p in Windows)
                {
                    if (WinApi.GetForegroundWindow() == p.Hwnd)
                    {
                        isActive = true;
                        break;
                    }
                }
                _isActiveWindow = isActive;*/
                return _isActiveWindow;
            }
            set
            {
                _isActiveWindow = value;
                NotifyPropertyChanged("_isActiveWindow");
            }
        }

        /*public static readonly DependencyProperty GroupTabModeProperty =
            DependencyProperty.Register("GroupTabMode", typeof(TabMode), typeof(TaskItemGroup), new FrameworkPropertyMetadata(TabMode.None, FrameworkPropertyMetadataOptions.AffectsRender));*/
        string _executableName = "";
        public string ExecutableName
        {
            get => _executableName;
            set
            {
                _executableName = value;
                NotifyPropertyChanged("ExecutableName");
            }
        }


        public ImageBrush Icon
        {
            get
            {
                if (File.Exists(ExecutableName))
                {
                    return new ImageBrush(MiscTools.GetIconFromFilePath(ExecutableName, 32, 32, (uint)(0x00000000 | 0x100)));
                }
                else
                {
                    return new ImageBrush();
                }
            }
        }

        public TaskItemGroup(string name)
        {
            ExecutableName = name;
            //DataContext = this;
            /*Binding countBinding = new Binding()
            {
                Source = this.Windows,
                Path = new PropertyPath("Count"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(this, TaskItemGroup.WindowCountProperty, countBinding);*/
            Windows.CollectionChanged += (sneder, args) =>
            {
                if (Windows.Count == 0)
                {
                    GroupTabMode = TabMode.NotRunning;
                }
                else if (Windows.Count == 1)
                {
                    GroupTabMode = TabMode.None;
                }
                else if (Windows.Count == 2)
                {
                    GroupTabMode = TabMode.Single;
                }
                else
                {
                    GroupTabMode = TabMode.Double;
                }

                //Debug.WriteLine(Windows.Count);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /*private void NotifyPropertyChanged()
        {
            NotifyPropertyChanged("");
        }
        */
        private void NotifyPropertyChanged(string propertyName)
        {
            if (propertyName == "_activeWindowItem")
            {
                if (ActiveWindowItem != null)
                {
                    if (Application.Current.MainWindow.IsLoaded)
                        ActiveWindowItem.Show();
                }
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
