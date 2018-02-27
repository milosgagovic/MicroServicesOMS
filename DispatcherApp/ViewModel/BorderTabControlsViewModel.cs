using DispatcherApp.Model;
using DispatcherApp.View;
using DispatcherApp.View.CustomControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DispatcherApp.ViewModel
{
    public class BorderTabControlsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private static ObservableCollection<BorderTabItem> leftTabControlTabs = new ObservableCollection<BorderTabItem>();
        private static int leftTabControlIndex = 0;
        private static Visibility leftTabControlVisibility = Visibility.Collapsed;

        private static ObservableCollection<TabItem> rightTabControlTabs = new ObservableCollection<TabItem>();
        private static int rightTabControlIndex = 0;
        private static Visibility rightTabControlVisibility = Visibility.Collapsed;

        private static ObservableCollection<TabItem> bottomTabControlTabs = new ObservableCollection<TabItem>();
        private static int bottomTabControlIndex = 0;
        private static Visibility bottomTabControlVisibility = Visibility.Collapsed;

        private NetworkExplorerControl networkExplorer = new NetworkExplorerControl();

        private RelayCommand _closeControlCommand;

        public RelayCommand CloseControlCommand
        {
            get
            {
                return _closeControlCommand ?? new RelayCommand(
                    (parameter) =>
                    {
                        ExecuteCloseControlCommand(parameter);
                    });
            }
        }

        private void ExecuteOpenControlCommand(object parameter)
        {
            if (parameter as string == "Network Explorer")
            {
                bool exists = false;
                int i = 0;

                for (i = 0; i < LeftTabControlTabs.Count; i++)
                {
                    if (LeftTabControlTabs[i].Header == parameter)
                    {
                        exists = true;
                        this.LeftTabControlIndex = i;
                        break;
                    }
                }

                if (!exists)
                {
                    BorderTabItem ti = new BorderTabItem() { Header = parameter };
                    ti.Scroll.Content = networkExplorer;
                    ti.Title.Text = (string)parameter;

                    if (!leftTabControlTabs.Contains(ti))
                    {
                        this.LeftTabControlTabs.Add(ti);
                        this.LeftTabControlIndex = this.LeftTabControlTabs.Count - 1;
                    }
                }

                this.LeftTabControlVisibility = Visibility.Visible;
            }
            //else if (parameter as string == "Properties")
            //{
            //    bool exists = false;
            //    int i = 0;

            //    for (i = 0; i < RightTabControlTabs.Count; i++)
            //    {
            //        if (RightTabControlTabs[i].Header == parameter)
            //        {
            //            exists = true;
            //            this.RightTabControlIndex = i;
            //            break;
            //        }
            //    }

            //    if (!exists)
            //    {
            //        BorderTabItem ti = new BorderTabItem() { Header = parameter };
            //        if (!RightTabControlTabs.Contains(ti))
            //        {
            //            ti.Title.Text = (string)parameter;
            //            SetTabContent(ti, null);
            //            this.RightTabControlTabs.Add(ti);
            //            this.RightTabControlIndex = this.RightTabControlTabs.Count - 1;
            //        }
            //    }

            //    this.RightTabControlVisibility = Visibility.Visible;
            //}
            //else if (parameter as string == "Incident Explorer" || parameter as string == "Output")
            //{
            //    bool exists = false;
            //    int i = 0;

            //    for (i = 0; i < BottomTabControlTabs.Count; i++)
            //    {
            //        if (BottomTabControlTabs[i].Header == parameter)
            //        {
            //            exists = true;
            //            this.BottomTabControlIndex = i;
            //            break;
            //        }
            //    }

            //    if (!exists)
            //    {
            //        BorderTabItem ti = new BorderTabItem() { Header = parameter };
            //        if (parameter as string == "Incident Explorer")
            //        {
            //            ti.Scroll.Content = incidentExplorer;
            //            ti.Title.Text = (string)parameter;
            //        }
            //        else if (parameter as string == "Output")
            //        {
            //            ti.Scroll.Content = output;
            //            ti.Title.Text = (string)parameter;
            //        }

            //        if (!BottomTabControlTabs.Contains(ti))
            //        {
            //            this.BottomTabControlTabs.Add(ti);
            //            this.BottomTabControlIndex = this.BottomTabControlTabs.Count - 1;
            //        }
            //    }

            //    this.BottomTabControlVisibility = Visibility.Visible;
            //}
            //else
            //{
            //    //bool exists = false;
            //    //int i = 0;
            //    //Element element = null;
            //    //if (parameter != null)
            //    //{
            //    //    Network.TryGetValue((long)parameter, out element);

            //    //    if (element != null)
            //    //    {
            //    //        for (i = 0; i < CenterTabControlTabs.Count; i++)
            //    //        {
            //    //            if (CenterTabControlTabs[i].Header as string == element.MRID)
            //    //            {
            //    //                exists = true;
            //    //                this.CenterTabControlIndex = i;
            //    //                break;
            //    //            }
            //    //        }
            //    //    }

            //    //    if (!exists)
            //    //    {
            //    //        TabItem ti = new TabItem()
            //    //        {
            //    //            Content = networModelControls[(long)parameter],
            //    //            Header = element.MRID
            //    //        };

            //    //        if (!CenterTabControlTabs.Contains(ti))
            //    //        {
            //    //            this.CenterTabControlTabs.Add(ti);
            //    //            this.CenterTabControlIndex = this.CenterTabControlTabs.Count - 1;
            //    //        }
            //    //    }
            //}
        }

        private void ExecuteCloseControlCommand(object parameter)
        {
            if ((string)parameter == "Network Explorer")
            {
                int i = 0;

                for (i = 0; i < LeftTabControlTabs.Count; i++)
                {
                    if ((string)LeftTabControlTabs[i].Header == (string)parameter)
                    {
                        LeftTabControlTabs[i].Content = null;
                        LeftTabControlTabs[i].Visibility = Visibility.Collapsed;
                        LeftTabControlTabs.RemoveAt(i);
                        break;
                    }
                }

                if (LeftTabControlTabs.Count == 0)
                {
                    this.LeftTabControlVisibility = Visibility.Collapsed;
                }
            }
            else if ((string)parameter == "Properties")
            {
                int i = 0;

                for (i = 0; i < RightTabControlTabs.Count; i++)
                {
                    if ((string)RightTabControlTabs[i].Header == (string)parameter)
                    {
                        RightTabControlTabs[i].Content = null;
                        RightTabControlTabs[i].Visibility = Visibility.Collapsed;
                        RightTabControlTabs.RemoveAt(i);
                        break;
                    }
                }

                if (RightTabControlTabs.Count == 0)
                {
                    this.RightTabControlVisibility = Visibility.Collapsed;
                }
            }
            else if ((string)parameter == "Incident Explorer" || (string)parameter == "Output")
            {
                int i = 0;

                for (i = 0; i < BottomTabControlTabs.Count; i++)
                {
                    if ((string)BottomTabControlTabs[i].Header == (string)parameter)
                    {
                        BottomTabControlTabs[i].Content = null;
                        BottomTabControlTabs[i].Visibility = Visibility.Collapsed;
                        BottomTabControlTabs.RemoveAt(i);
                        break;
                    }
                }

                if (BottomTabControlTabs.Count == 0)
                {
                    this.BottomTabControlVisibility = Visibility.Collapsed;
                }
            }
        }

        public ObservableCollection<BorderTabItem> LeftTabControlTabs
        {
            get
            {
                return leftTabControlTabs;
            }
            set
            {
                leftTabControlTabs = value;
            }
        }

        public int LeftTabControlIndex
        {
            get
            {
                return leftTabControlIndex;
            }
            set
            {
                leftTabControlIndex = value;
                RaisePropertyChanged("LeftTabControlIndex");
            }
        }

        public ObservableCollection<TabItem> RightTabControlTabs
        {
            get
            {
                return rightTabControlTabs;
            }
            set
            {
                rightTabControlTabs = value;
            }
        }

        public int RightTabControlIndex
        {
            get
            {
                return rightTabControlIndex;
            }
            set
            {
                rightTabControlIndex = value;
                RaisePropertyChanged("RightTabControlIndex");
            }
        }

        public ObservableCollection<TabItem> BottomTabControlTabs
        {
            get
            {
                return bottomTabControlTabs;
            }
            set
            {
                bottomTabControlTabs = value;
            }
        }

        public int BottomTabControlIndex
        {
            get
            {
                return bottomTabControlIndex;
            }
            set
            {
                bottomTabControlIndex = value;
                RaisePropertyChanged("BottomTabControlIndex");
            }
        }

        public Visibility LeftTabControlVisibility
        {
            get
            {
                return leftTabControlVisibility;
            }
            set
            {
                leftTabControlVisibility = value;
                RaisePropertyChanged("LeftTabControlVisibility");
            }
        }

        public Visibility RightTabControlVisibility
        {
            get
            {
                return rightTabControlVisibility;
            }
            set
            {
                rightTabControlVisibility = value;
                RaisePropertyChanged("RightTabControlVisibility");
            }
        }

        public Visibility BottomTabControlVisibility
        {
            get
            {
                return bottomTabControlVisibility;
            }
            set
            {
                bottomTabControlVisibility = value;
                RaisePropertyChanged("BottomTabControlVisibility");
            }
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
