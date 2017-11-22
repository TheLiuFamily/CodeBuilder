using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

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

		private readonly ObservableCollection<TreeNode> children;
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

			children = new ObservableCollection<TreeNode>();

			if (lazyLoadChildren)
				children.Add(DummyChild);
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
		public ObservableCollection<TreeNode> Children
		{
			get { return children; }
		}

		/// <summary>
		/// Returns true if this object's Children have not yet been populated.
		/// </summary>
		public bool HasDummyChild
		{
			get { return Children.Count == 1 && Children[0] == DummyChild; }
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
						Children.Remove(DummyChild);
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
				Children.Add(new TreeNode(this, true) { DisplayName = "subnode " + i });
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
}