using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;

namespace NewDock
{
	public abstract class DockPaneStripBase : Control
	{
	    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]        
        protected sealed class TabCollection : IEnumerable<DockPaneStripTabBase>
        {
            #region IEnumerable Members
            IEnumerator<DockPaneStripTabBase> IEnumerable<DockPaneStripTabBase>.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }
            #endregion

            public TabCollection(DockPane pane)
            {
                m_dockPane = pane;
            }

            private DockPane m_dockPane;
            public DockPane DockPane
            {
                get { return m_dockPane; }
            }

            public int Count
            {
                get { return DockPane.DisplayingContents.Count; }
            }

            public DockPaneStripTabBase this[int index]
            {
                get
                {
                    IDockContent content = DockPane.DisplayingContents[index];
                    if (content == null)
                        throw (new ArgumentOutOfRangeException("index"));
                    return content.DockHandler.GetTab(DockPane.TabStripControl);
                }
            }

            public bool Contains(DockPaneStripTabBase tab)
            {
                return (IndexOf(tab) != -1);
            }

            public bool Contains(IDockContent content)
            {
                return (IndexOf(content) != -1);
            }

            public int IndexOf(DockPaneStripTabBase tab)
            {
                if (tab == null)
                    return -1;

                return DockPane.DisplayingContents.IndexOf(tab.Content);
            }

            public int IndexOf(IDockContent content)
            {
                return DockPane.DisplayingContents.IndexOf(content);
            }
        }

		protected DockPaneStripBase(DockPane pane)
		{
			m_dockPane = pane;

			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, false);
            AllowDrop = true;
		}

		private DockPane m_dockPane;
		protected DockPane DockPane
		{
			get	{	return m_dockPane;	}
		}

		protected DockPaneAppearanceStyle Appearance
		{
			get	{	return DockPane.Appearance;	}
		}

        private TabCollection m_tabs = null;
		protected TabCollection Tabs
		{
			get
            {
                if (m_tabs == null)
                    m_tabs = new TabCollection(DockPane);

                return m_tabs;
            }
		}

		public void RefreshChanges()
		{
            if (IsDisposed)
                return;

			OnRefreshChanges();
		}

		protected virtual void OnRefreshChanges()
		{
		}

		public abstract int MeasureHeight();

		public abstract void EnsureTabVisible(IDockContent content);

		protected int HitTest()
		{
			return HitTest(PointToClient(Control.MousePosition));
		}

		public abstract int HitTest(Point point);

		public abstract GraphicsPath GetOutline(int index);

        public virtual DockPaneStripTabBase CreateTab(IDockContent content)
        {
            return new DockPaneStripTabBase(content);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            int index = HitTest();
            if (index != -1)
            {
                IDockContent content = Tabs[index].Content;
                if (DockPane.ActiveContent != content)
                    DockPane.ActiveContent = content;
            }

            if (e.Button == MouseButtons.Left)
            {
                if (DockPane.DockPanel.AllowEndUserDocking && DockPane.AllowDockDragAndDrop && DockPane.ActiveContent.DockHandler.AllowEndUserDocking)
                    DockPane.DockPanel.BeginDrag(DockPane.ActiveContent.DockHandler);
            }
        }

        protected bool HasTabPageContextMenu
        {
            get { return DockPane.HasTabPageContextMenu; }
        }

        protected void ShowTabPageContextMenu(Point position)
        {
            DockPane.ShowTabPageContextMenu(this, position);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButtons.Right)
                ShowTabPageContextMenu(new Point(e.X, e.Y));
            else if ((e.Button == MouseButtons.Middle) && (DockPane.Appearance == Docking.DockPane.AppearanceStyle.Document))
            {
                // Get the content located under the click (if there is one)
                int index = HitTest();
                if (index != -1)
                {
                    // Close the specified content.
                    IDockContent content = Tabs[index].Content;
                    DockPane.CloseContent(content);
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
			{
				base.WndProc(ref m);

				int index = HitTest();
				if (DockPane.DockPanel.AllowEndUserDocking && index != -1)
				{
					IDockContent content = Tabs[index].Content;
                    if (content.DockHandler.CheckDockState(!content.DockHandler.IsFloat) != DockState.Unknown)
					    content.DockHandler.IsFloat = !content.DockHandler.IsFloat;	
				}

				return;
			}

			base.WndProc(ref m);
			return;
		}

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            int index = HitTest();
            if (index != -1)
            {
                IDockContent content = Tabs[index].Content;
                if (DockPane.ActiveContent != content)
                    DockPane.ActiveContent = content;
            }
        }
	}
}
