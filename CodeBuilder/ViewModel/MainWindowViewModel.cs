using CodeBuilder.ExampleViews;
using CodeBuilder.Models;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CodeBuilder.ViewModel
{
	/// <summary>
	/// Sample base class for tree items view models. All specialised tree item view model classes
	/// should inherit from this class.
	/// </summary>
	[Obfuscation(Exclude = true, ApplyToMembers = false, Feature = "renaming")]
	public class TreeNode : INotifyPropertyChanged
	{
		#region Data

		private static readonly TreeNode DummyChild = new TreeNode();

		private readonly ObservableCollection<TreeNode> nodes;
		private readonly TreeNode parent;

		private bool isExpanded;
		private bool isSelected;
		private bool isEditable;
		private bool isEditing;
		private bool isEnabled = true;
		private bool isVisible = true;
		private string remarks;

		#endregion Data

		#region Constructor

		public TreeNode(TreeNode parent, bool lazyLoadChildren)
		{
			this.parent = parent;

			nodes = new ObservableCollection<TreeNode>();

			if (lazyLoadChildren)
				nodes.Add(DummyChild);
		}

        // This is used to create the DummyChild instance.
        public TreeNode()
            : this(null, false)
		{
		}

		#endregion Constructor

		#region Public properties

		/// <summary>
		/// Returns the logical child items of this object.
		/// </summary>
		public ObservableCollection<TreeNode> Nodes
		{
			get { return nodes; }
		}

		/// <summary>
		/// Returns true if this object's Children have not yet been populated.
		/// </summary>
		public bool HasDummyChild
		{
			get { return Nodes.Count == 1 && Nodes[0] == DummyChild; }
		}

		/// <summary>
		/// Gets/sets whether the TreeViewItem 
		/// associated with this object is expanded.
		/// </summary>
		public bool IsExpanded
		{
			get { return isExpanded; }
			set
			{
				if (value != isExpanded)
				{
					isExpanded = value;
					OnPropertyChanged("IsExpanded");

					// Expand all the way up to the root.
					if (isExpanded && parent != null)
						parent.IsExpanded = true;

					// Lazy load the child items, if necessary.
					if (isExpanded && HasDummyChild)
					{
						Nodes.Remove(DummyChild);
						LoadChildren();
					}
				}
			}
		}

		/// <summary>
		/// Invoked when the child items need to be loaded on demand.
		/// Subclasses can override this to populate the Children collection.
		/// </summary>
		protected virtual void LoadChildren()
		{
			for (int i = 0; i < 100; i++)
			{
				Nodes.Add(new TreeNode(this, true) { DisplayName = "subnode " + i });
			}
		}

		/// <summary>
		/// Gets/sets whether the TreeViewItem 
		/// associated with this object is selected.
		/// </summary>
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				if (value != isSelected)
				{
					isSelected = value;
					OnPropertyChanged("IsSelected");
				}
			}
		}

		public bool IsEditable
		{
			get { return isEditable; }
			set
			{
				if (value != isEditable)
				{
					isEditable = value;
					OnPropertyChanged("IsEditable");
				}
			}
		}

		public bool IsEditing
		{
			get { return isEditing; }
			set
			{
				if (value != isEditing)
				{
					isEditing = value;
					OnPropertyChanged("IsEditing");
				}
			}
		}

		public bool IsEnabled
		{
			get { return isEnabled; }
			set
			{
				if (value != isEnabled)
				{
					isEnabled = value;
					OnPropertyChanged("IsEnabled");
				}
			}
		}

		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				if (value != isVisible)
				{
					isVisible = value;
					OnPropertyChanged("IsVisible");
				}
			}
		}

		public string Remarks
		{
			get { return remarks; }
			set
			{
				if (value != remarks)
				{
					remarks = value;
					OnPropertyChanged("Remarks");
				}
			}
		}

		public TreeNode Parent
		{
			get { return parent; }
		}

		public override string ToString()
		{
			return "{Node " + DisplayName + "}";
		}

		#endregion Public properties

		#region ViewModelBase

		/// <summary>
		/// Returns the user-friendly name of this object.
		/// Child classes can set this property to a new value,
		/// or override it to determine the value on-demand.
		/// </summary>
		private string displayName;
		public virtual string DisplayName
        {
			get { return displayName; }
			set
			{
				if (value != displayName)
				{
                    displayName = value;
					OnPropertyChanged("DisplayName");
				}
			}
		}
        private string text;
        public virtual string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        private string icon;
        public virtual string Icon
        {
            get { return icon; }
            set
            {
                if (value != icon)
                {
                    icon = value;
                    OnPropertyChanged("Icon");
                }
            }
        }

        private object tag;
        public virtual object Tag
        {
            get { return tag; }
            set
            {
                if (value != tag)
                {
                    tag = value;
                    OnPropertyChanged("Tag");
                }
            }
        }


        #endregion ViewModelBase

        #region INotifyPropertyChanged members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raises this object's PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">The property that has a new value.</param>
		protected void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion INotifyPropertyChanged members
	}

    public class AccentColorMenuData
    {

        protected static string AppTheme { get; set; }
        protected static string Accent { get; set; }
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand { CanExecuteDelegate = x => true, ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            ChangeTheme(AppTheme, Name);
        }

        public static void ChangeTheme(string themeStr, string accentStr)
        {
            AppTheme = themeStr;
            Accent = accentStr;
            Accent accent = ThemeManager.GetAccent(accentStr);
            AppTheme theme = ThemeManager.GetAppTheme(themeStr);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme);

            //记录换肤，下次启动直接使用
            Configuration cf = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cf.AppSettings.Settings["Skin"].Value = accentStr;
            cf.AppSettings.Settings["Theme"].Value = themeStr;
            cf.Save();
        }

    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            ChangeTheme(Name, Accent);
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged, IDataErrorInfo, IDisposable
    {
      
		public ObservableCollection<TreeNode> Nodes
        {
            get { return nodes; }
        }
        private readonly ObservableCollection<TreeNode> nodes = new ObservableCollection<TreeNode>();
        private readonly IDialogCoordinator _dialogCoordinator;
        int? _integerGreater10Property;
        private bool _animateOnPositionChange = true;

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator)
        {
            this.Title = "Flyout Binding Test";
            _dialogCoordinator = dialogCoordinator;
            SampleData.Seed();

            // create accent color menu items for the demo
            this.AccentColors = ThemeManager.Accents
                                            .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                            .ToList();

            // create metro theme color menu items for the demo
            this.AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();


            Albums = SampleData.Albums;
            Artists = SampleData.Artists;

            FlipViewImages = new Uri[]
                             {
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-4/landscapes/mountains/painted-desert.jpg", UriKind.Absolute),
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-3/landscapes/forest/breaking-the-clouds-on-winter-day.jpg", UriKind.Absolute),
                                 new Uri("http://www.public-domain-photos.com/free-stock-photos-4/travel/bodie/bodie-streets.jpg", UriKind.Absolute)
                             };

            BrushResources = FindBrushResources();

            CultureInfos = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures).OrderBy(c => c.DisplayName).ToList();

            try
            {
                HotkeyManager.Current.AddOrReplace("demo", HotKey.Key, HotKey.ModifierKeys, (sender, e) => OnHotKey(sender, e));
            }
            catch (HotkeyAlreadyRegisteredException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Uups, the hotkey {0} is already registered!", exception.Name);
            }
        }

        public void Dispose()
        {
            HotkeyManager.Current.Remove("demo");
        }

        public string Title { get; set; }
        public int SelectedIndex { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> Artists { get; set; }
        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }
        public List<CultureInfo> CultureInfos { get; set; }

        public int? IntegerGreater10Property
        {
            get { return this._integerGreater10Property; }
            set
            {
                if (Equals(value, _integerGreater10Property))
                {
                    return;
                }

                _integerGreater10Property = value;
                RaisePropertyChanged("IntegerGreater10Property");
            }
        }

        DateTime? _datePickerDate;

        [Display(Prompt = "Auto resolved Watermark")]
        public DateTime? DatePickerDate
        {
            get { return this._datePickerDate; }
            set
            {
                if (Equals(value, _datePickerDate))
                {
                    return;
                }

                _datePickerDate = value;
                RaisePropertyChanged("DatePickerDate");
            }
        }

        bool _magicToggleButtonIsChecked = true;
        public bool MagicToggleButtonIsChecked
        {
            get { return this._magicToggleButtonIsChecked; }
            set
            {
                if (Equals(value, _magicToggleButtonIsChecked))
                {
                    return;
                }

                _magicToggleButtonIsChecked = value;
                RaisePropertyChanged("MagicToggleButtonIsChecked");
            }
        }

        private bool _quitConfirmationEnabled;
        public bool QuitConfirmationEnabled
        {
            get { return _quitConfirmationEnabled; }
            set
            {
                if (value.Equals(_quitConfirmationEnabled)) return;
                _quitConfirmationEnabled = value;
                RaisePropertyChanged("QuitConfirmationEnabled");
            }
        }

        private bool showMyTitleBar = true;
        public bool ShowMyTitleBar
        {
            get { return showMyTitleBar; }
            set
            {
                if (value.Equals(showMyTitleBar)) return;
                showMyTitleBar = value;
                RaisePropertyChanged("ShowMyTitleBar");
            }
        }

        private bool canCloseFlyout = true;

        public bool CanCloseFlyout
        {
            get { return this.canCloseFlyout; }
            set
            {
                if (Equals(value, this.canCloseFlyout))
                {
                    return;
                }
                this.canCloseFlyout = value;
                this.RaisePropertyChanged("CanCloseFlyout");
            }
        }

        private ICommand closeCmd;

        public ICommand CloseCmd
        {
            get
            {
                return this.closeCmd ?? (this.closeCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => this.CanCloseFlyout,
                    ExecuteDelegate = x => ((Flyout)x).IsOpen = false
                });
            }
        }

        private bool canShowHamburgerAboutCommand = true;

        public bool CanShowHamburgerAboutCommand
        {
            get { return this.canShowHamburgerAboutCommand; }
            set
            {
                if (Equals(value, this.canShowHamburgerAboutCommand))
                {
                    return;
                }
                this.canShowHamburgerAboutCommand = value;
                this.RaisePropertyChanged("CanShowHamburgerAboutCommand");
            }
        }

        private bool isHamburgerMenuPaneOpen;

        public bool IsHamburgerMenuPaneOpen
        {
            get { return this.isHamburgerMenuPaneOpen; }
            set
            {
                if (Equals(value, this.isHamburgerMenuPaneOpen))
                {
                    return;
                }
                this.isHamburgerMenuPaneOpen = value;
                this.RaisePropertyChanged("IsHamburgerMenuPaneOpen");
            }
        }

        private ICommand textBoxButtonCmd;

        public ICommand TextBoxButtonCmd
        {
            get
            {
                return this.textBoxButtonCmd ?? (this.textBoxButtonCmd = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        if (x is string)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Wow, you typed Return and got", (string)x);
                        }
                        else if (x is TextBox)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("TextBox Button was clicked!", string.Format("Text: {0}", ((TextBox)x).Text));
                        }
                        else if (x is PasswordBox)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("PasswordBox Button was clicked!", string.Format("Password: {0}", ((PasswordBox)x).Password));
                        }
                    }
                });
            }
        }

        private ICommand textBoxButtonCmdWithParameter;

        public ICommand TextBoxButtonCmdWithParameter
        {
            get
            {
                return this.textBoxButtonCmdWithParameter ?? (this.textBoxButtonCmdWithParameter = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        if (x is String)
                        {
                            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("TextBox Button with parameter was clicked!",
                                                                                                  string.Format("Parameter: {0}", x));
                        }
                    }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event if needed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "IntegerGreater10Property" && this.IntegerGreater10Property < 10)
                {
                    return "Number is not greater than 10!";
                }

                if (columnName == "DatePickerDate" && this.DatePickerDate == null)
                {
                    return "No date given!";
                }

                if (columnName == "HotKey" && this.HotKey != null && this.HotKey.Key == Key.D && this.HotKey.ModifierKeys == ModifierKeys.Shift)
                {
                    return "SHIFT-D is not allowed";
                }

                return null;
            }
        }

        [Description("Test-Property")]
        public string Error { get { return string.Empty; } }

        private ICommand singleCloseTabCommand;

        public ICommand SingleCloseTabCommand
        {
            get
            {
                return this.singleCloseTabCommand ?? (this.singleCloseTabCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync("Closing tab!", string.Format("You are now closing the '{0}' tab", x));
                    }
                });
            }
        }

        private ICommand neverCloseTabCommand;

        public ICommand NeverCloseTabCommand
        {
            get { return this.neverCloseTabCommand ?? (this.neverCloseTabCommand = new SimpleCommand { CanExecuteDelegate = x => false }); }
        }


        private ICommand showInputDialogCommand;

        public ICommand ShowInputDialogCommand
        {
            get
            {
                return this.showInputDialogCommand ?? (this.showInputDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        await _dialogCoordinator.ShowInputAsync(this, "From a VM", "This dialog was shown from a VM, without knowledge of Window").ContinueWith(t => Console.WriteLine(t.Result));
                    }
                });
            }
        }

        private ICommand showLoginDialogCommand;

        public ICommand ShowLoginDialogCommand
        {
            get
            {
                return this.showLoginDialogCommand ?? (this.showLoginDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = async x =>
                    {
                        await _dialogCoordinator.ShowLoginAsync(this, "Login from a VM", "This login dialog was shown from a VM, so you can be all MVVM.").ContinueWith(t => Console.WriteLine(t.Result));
                    }
                });
            }
        }

        private ICommand showMessageDialogCommand;

        public ICommand ShowMessageDialogCommand
        {
            get
            {
                return this.showMessageDialogCommand ?? (this.showMessageDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => PerformDialogCoordinatorAction(this.ShowMessage((string)x), (string)x == "DISPATCHER_THREAD")
                });
            }
        }

        private Action ShowMessage(string startingThread)
        {
            return () =>
            {
                var message = $"MVVM based messages!\n\nThis dialog was created by {startingThread} Thread with ID=\"{Thread.CurrentThread.ManagedThreadId}\"\n" +
                              $"The current DISPATCHER_THREAD Thread has the ID=\"{Application.Current.Dispatcher.Thread.ManagedThreadId}\"";
                this._dialogCoordinator.ShowMessageAsync(this, $"Message from VM created by {startingThread}", message).ContinueWith(t => Console.WriteLine(t.Result));
            };
        }

        private ICommand showProgressDialogCommand;

        public ICommand ShowProgressDialogCommand
        {
            get
            {
                return this.showProgressDialogCommand ?? (this.showProgressDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunProgressFromVm()
                });
            }
        }

        private async void RunProgressFromVm()
        {
            var controller = await _dialogCoordinator.ShowProgressAsync(this, "Progress from VM", "Progressing all the things, wait 3 seconds");
            controller.SetIndeterminate();

            await Task.Delay(3000);

            await controller.CloseAsync();
        }

        private static void PerformDialogCoordinatorAction(Action action, bool runInMainThread)
        {
            if (!runInMainThread)
            {
                Task.Factory.StartNew(action);
            }
            else
            {
                action();
            }
        }


        private ICommand showCustomDialogCommand;

        public ICommand ShowCustomDialogCommand
        {
            get
            {
                return this.showCustomDialogCommand ?? (this.showCustomDialogCommand = new SimpleCommand
                {
                    CanExecuteDelegate = x => true,
                    ExecuteDelegate = x => RunCustomFromVm()
                });
            }
        }

        private async void RunCustomFromVm()
        {
            var customDialog = new CustomDialog() { Title = "Custom Dialog" };

            var customDialogExampleContent = new CustomDialogExampleContent(instance =>
            {
                _dialogCoordinator.HideMetroDialogAsync(this, customDialog);
                System.Diagnostics.Debug.WriteLine(instance.FirstName);
            });
            customDialog.Content = new CustomDialogExample { DataContext = customDialogExampleContent };

            await _dialogCoordinator.ShowMetroDialogAsync(this, customDialog);
        }

        public IEnumerable<string> BrushResources { get; private set; }

        public bool AnimateOnPositionChange
        {
            get
            {
                return _animateOnPositionChange;
            }
            set
            {
                if (Equals(_animateOnPositionChange, value)) return;
                _animateOnPositionChange = value;
                RaisePropertyChanged("AnimateOnPositionChange");
            }
        }

        private IEnumerable<string> FindBrushResources()
        {
            var rd = new ResourceDictionary
            {
                Source = new Uri(@"/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute)
            };

            var resources = rd.Keys.Cast<object>()
                    .Where(key => rd[key] is Brush)
                    .Select(key => key.ToString())
                    .OrderBy(s => s)
                    .ToList();

            return resources;
        }

        public Uri[] FlipViewImages
        {
            get;
            set;
        }


        public class RandomDataTemplateSelector : DataTemplateSelector
        {
            public DataTemplate TemplateOne { get; set; }

            public override DataTemplate SelectTemplate(object item, DependencyObject container)
            {
                return TemplateOne;
            }
        }

        private HotKey _hotKey = new HotKey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift);

        public HotKey HotKey
        {
            get { return _hotKey; }
            set
            {
                if (_hotKey != value)
                {
                    _hotKey = value;
                    if (_hotKey != null && _hotKey.Key != Key.None)
                    {
                        HotkeyManager.Current.AddOrReplace("demo", HotKey.Key, HotKey.ModifierKeys, (sender, e) => OnHotKey(sender, e));
                    }
                    else
                    {
                        HotkeyManager.Current.Remove("demo");
                    }
                    RaisePropertyChanged("HotKey");
                }
            }
        }

        private async Task OnHotKey(object sender, HotkeyEventArgs e)
        {
            await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(
                "Hotkey pressed",
                "You pressed the hotkey '" + HotKey + "' registered with the name '" + e.Name + "'");
        }

        private ICommand toggleIconScalingCommand;

        public ICommand ToggleIconScalingCommand
        {
            get
            {
                return toggleIconScalingCommand ?? (toggleIconScalingCommand = new SimpleCommand
                {
                    ExecuteDelegate = ToggleIconScaling
                });
            }
        }

        private void ToggleIconScaling(object obj)
        {
            var multiFrameImageMode = (MultiFrameImageMode)obj;
            ((MetroWindow)Application.Current.MainWindow).IconScalingMode = multiFrameImageMode;
            RaisePropertyChanged("IsScaleDownLargerFrame");
            RaisePropertyChanged("IsNoScaleSmallerFrame");
        }

        public bool IsScaleDownLargerFrame { get { return ((MetroWindow)Application.Current.MainWindow).IconScalingMode == MultiFrameImageMode.ScaleDownLargerFrame; } }

        public bool IsNoScaleSmallerFrame { get { return ((MetroWindow)Application.Current.MainWindow).IconScalingMode == MultiFrameImageMode.NoScaleSmallerFrame; } }
    }
}