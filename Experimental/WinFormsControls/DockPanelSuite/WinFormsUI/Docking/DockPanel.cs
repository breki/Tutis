using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "0#")]
	public delegate IDockContent DeserializeDockContent(string persistString);

    [LocalizedDescription("DockPanel_Description")]
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    [ToolboxBitmap(typeof(resfinder), "WeifenLuo.WinFormsUI.Docking.DockPanel.bmp")]
    [DefaultProperty("DocumentStyle")]
    [DefaultEvent("ActiveContentChanged")]
	public partial class DockPanel : Panel
	{      
		public DockPanel()
		{
            focusManager = new FocusManagerImpl(this);
			extender = new DockPanelExtender(this);
			panes = new DockPaneCollection();
			floatWindows = new FloatWindowCollection();

            SuspendLayout();

			autoHideWindow = new AutoHideWindowControl(this);
			autoHideWindow.Visible = false;
            SetAutoHideWindowParent();

			dummyControl = new DummyControl();
			dummyControl.Bounds = new Rectangle(0, 0, 1, 1);
			Controls.Add(dummyControl);

			dockWindows = new DockWindowCollection(this);
			Controls.AddRange(new Control[]	{
				DockWindows[DockState.Document],
				DockWindows[DockState.DockLeft],
				DockWindows[DockState.DockRight],
				DockWindows[DockState.DockTop],
				DockWindows[DockState.DockBottom]
				});

			dummyContent = new DockContent();
            ResumeLayout();
        }

        [Browsable(false)]
        public IDockContent ActiveAutoHideContent
        {
            get	{	return AutoHideWindow.ActiveContent;	}
            set	{	AutoHideWindow.ActiveContent = value;	}
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_AllowEndUserNestedDocking_Description")]
        [DefaultValue(true)]
        public bool AllowEndUserNestedDocking
        {
            get { return m_allowEndUserNestedDocking; }
            set { m_allowEndUserNestedDocking = value; }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_AllowEndUserDocking_Description")]
        [DefaultValue(true)]
        public bool AllowEndUserDocking
        {
            get	{	return m_allowEndUserDocking;	}
            set	{	m_allowEndUserDocking = value;	}
        }

        [Browsable(false)]
        public DockContentCollection Contents
        {
            get	{	return m_contents;	}
        }

        [Category("Layout")]
        [LocalizedDescription("DockPanel_DefaultFloatWindowSize_Description")]
        public Size DefaultFloatWindowSize
        {
            get { return m_defaultFloatWindowSize; }
            set { m_defaultFloatWindowSize = value; }
        }

        /// <summary>
        /// Determines the color with which the client rectangle will be drawn.
        /// If you take this property instead of the BackColor it will not have any influence on the borders to the surrounding controls (DockPane).
        /// If you use BackColor the borders to the surrounding controls (DockPane) will also change there colors.
        /// Alternatively you can use both of them (BackColor to draw the define the color of the borders and DockBackColor to define the color of the client rectangle). 
        /// For Backgroundimages: Set your prefered Image, then set the DockBackColor and the BackColor to the same Color (Control)
        /// </summary>
        public Color DockBackColor
        {
            get
            {
                return !backColor.IsEmpty ? backColor : base.BackColor;
            }
            set
            {
                backColor = value;
            }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockBottomPortion_Description")]
        [DefaultValue(0.25)]
        public double DockBottomPortion
        {
            get	{	return m_dockBottomPortion;	}
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockBottomPortion)
                    return;

                m_dockBottomPortion = value;

                if (m_dockBottomPortion < 1 && m_dockTopPortion < 1)
                {
                    if (m_dockTopPortion + m_dockBottomPortion > 1)
                        m_dockTopPortion = 1 - m_dockBottomPortion;
                }

                PerformLayout();
            }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockLeftPortion_Description")]
        [DefaultValue(0.25)]
        public double DockLeftPortion
        {
            get	{	return m_dockLeftPortion;	}
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockLeftPortion)
                    return;

                m_dockLeftPortion = value;

                if (m_dockLeftPortion < 1 && m_dockRightPortion < 1)
                {
                    if (m_dockLeftPortion + m_dockRightPortion > 1)
                        m_dockRightPortion = 1 - m_dockLeftPortion;
                }
                PerformLayout();
            }
        }

        public DockPanelExtender.IDockPaneFactory DockPaneFactory
        {
            get	{	return Extender.DockPaneFactory;	}
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockRightPortion_Description")]
        [DefaultValue(0.25)]
        public double DockRightPortion
        {
            get	{	return m_dockRightPortion;	}
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockRightPortion)
                    return;

                m_dockRightPortion = value;

                if (m_dockLeftPortion < 1 && m_dockRightPortion < 1)
                {
                    if (m_dockLeftPortion + m_dockRightPortion > 1)
                        m_dockLeftPortion = 1 - m_dockRightPortion;
                }
                PerformLayout();
            }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockTopPortion_Description")]
        [DefaultValue(0.25)]
        public double DockTopPortion
        {
            get	{	return m_dockTopPortion;	}
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");

                if (value == m_dockTopPortion)
                    return;

                m_dockTopPortion = value;

                if (m_dockTopPortion < 1 && m_dockBottomPortion < 1)
                {
                    if (m_dockTopPortion + m_dockBottomPortion > 1)
                        m_dockBottomPortion = 1 - m_dockTopPortion;
                }
                PerformLayout();
            }
        }

        [Browsable(false)]
        public DockWindowCollection DockWindows
        {
            get	{	return dockWindows;	}
        }

        public IEnumerable<IDockContent> Documents
        {
            get
            {
                foreach (IDockContent content in Contents)
                {
                    if (content.DockHandler.DockState == DockState.Document)
                        yield return content;
                }
            }
        }

        public int DocumentsCount
        {
            get
            {
                int count = 0;
                foreach (IDockContent content in Documents)
                    count++;

                return count;
            }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DocumentStyle_Description")]
        [DefaultValue(DocumentStyle.DockingMdi)]
        public DocumentStyle DocumentStyle
        {
            get	{	return m_documentStyle;	}
            set
            {
                if (value == m_documentStyle)
                    return;

                if (!Enum.IsDefined(typeof(DocumentStyle), value))
                    throw new InvalidEnumArgumentException();

                if (value == DocumentStyle.SystemMdi && DockWindows[DockState.Document].VisibleNestedPanes.Count > 0)
                    throw new InvalidEnumArgumentException();

                m_documentStyle = value;

                SuspendLayout(true);

                SetAutoHideWindowParent();
                SetMdiClient();
                InvalidateWindowRegion();

                foreach (IDockContent content in Contents)
                {
                    if (content.DockHandler.DockState == DockState.Document)
                        content.DockHandler.SetPaneAndVisible(content.DockHandler.Pane);
                }

                PerformMdiClientLayout();

                ResumeLayout(true, true);
            }
        }

        [DefaultValue(DocumentTabStripLocation.Top)]
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DocumentTabStripLocation")]
        public DocumentTabStripLocation DocumentTabStripLocation
        {
            get { return m_documentTabStripLocation; }
            set { m_documentTabStripLocation = value; }
        }

        [Browsable(false)]
        public DockPanelExtender Extender
        {
            get	{	return extender;	}
        }

        public DockPanelExtender.IFloatWindowFactory FloatWindowFactory
        {
            get	{	return Extender.FloatWindowFactory;	}
        }

        [Browsable(false)]
        public FloatWindowCollection FloatWindows
        {
            get	{	return floatWindows;	}
        }

        [Browsable(false)]
        public DockPaneCollection Panes
        {
            get	{	return panes;	}
        }

        [DefaultValue(false)]
        [LocalizedCategory("Appearance")]
        [LocalizedDescription("DockPanel_RightToLeftLayout_Description")]
        public bool RightToLeftLayout
        {
            get { return m_rightToLeftLayout; }
            set
            {
                if (m_rightToLeftLayout == value)
                    return;

                m_rightToLeftLayout = value;
                foreach (FloatWindow floatWindow in FloatWindows)
                    floatWindow.RightToLeftLayout = value;
            }
        }

        [DefaultValue(false)]
        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_ShowDocumentIcon_Description")]
        public bool ShowDocumentIcon
        {
            get	{	return m_showDocumentIcon;	}
            set
            {
                if (m_showDocumentIcon == value)
                    return;

                m_showDocumentIcon = value;
                Refresh();
            }
        }

        [LocalizedCategory("Category_Docking")]
        [LocalizedDescription("DockPanel_DockPanelSkin")]
        public DockPanelSkin Skin
        {
            get { return m_dockPanelSkin; }
            set { m_dockPanelSkin = value; }
        }

        public IDockContent[] DocumentsToArray()
        {
            int count = DocumentsCount;
            IDockContent[] documents = new IDockContent[count];
            int i = 0;
            foreach (IDockContent content in Documents)
            {
                documents[i] = content;
                i++;
            }

            return documents;
        }

        public void SetPaneIndex(DockPane pane, int index)
        {
            int oldIndex = Panes.IndexOf(pane);
            if (oldIndex == -1)
                throw(new ArgumentException(Strings.DockPanel_SetPaneIndex_InvalidPane));

            if (index < 0 || index > Panes.Count - 1)
                if (index != -1)
                    throw(new ArgumentOutOfRangeException(Strings.DockPanel_SetPaneIndex_InvalidIndex));
				
            if (oldIndex == index)
                return;
            if (oldIndex == Panes.Count - 1 && index == -1)
                return;

            Panes.Remove(pane);
            if (index == -1)
                Panes.Add(pane);
            else if (oldIndex < index)
                Panes.AddAt(pane, index - 1);
            else
                Panes.AddAt(pane, index);
        }

        public void UpdateDockWindowZOrder(DockStyle dockStyle, bool fullPanelEdge)
        {
            if (dockStyle == DockStyle.Left)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockLeft].SendToBack();
                else
                    DockWindows[DockState.DockLeft].BringToFront();
            }
            else if (dockStyle == DockStyle.Right)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockRight].SendToBack();
                else
                    DockWindows[DockState.DockRight].BringToFront();
            }
            else if (dockStyle == DockStyle.Top)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockTop].SendToBack();
                else
                    DockWindows[DockState.DockTop].BringToFront();
            }
            else if (dockStyle == DockStyle.Bottom)
            {
                if (fullPanelEdge)
                    DockWindows[DockState.DockBottom].SendToBack();
                else
                    DockWindows[DockState.DockBottom].BringToFront();
            }
        }

        [LocalizedCategory("Category_DockingNotification")]
        [LocalizedDescription("DockPanel_ContentAdded_Description")]
        public event EventHandler<DockContentEventArgs> ContentAdded
        {
            add	{	Events.AddHandler(ContentAddedEvent, value);	}
            remove	{	Events.RemoveHandler(ContentAddedEvent, value);	}
        }

        [LocalizedCategory("Category_DockingNotification")]
        [LocalizedDescription("DockPanel_ContentRemoved_Description")]
        public event EventHandler<DockContentEventArgs> ContentRemoved
        {
            add	{	Events.AddHandler(ContentRemovedEvent, value);	}
            remove	{	Events.RemoveHandler(ContentRemovedEvent, value);	}
        }

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (!disposed && disposing)
                {
                    focusManager.Dispose();
                    if (m_mdiClientController != null)
                    {
                        m_mdiClientController.HandleAssigned -= MdiClientHandleAssigned;
                        m_mdiClientController.MdiChildActivate -= ParentFormMdiChildActivate;
                        m_mdiClientController.Layout -= MdiClient_Layout;
                        m_mdiClientController.Dispose();
                    }

                    FloatWindows.Dispose();
                    Panes.Dispose();
                    DummyContent.Dispose();

                    disposed = true;
                }
				
                base.Dispose(disposing);
            }
        }

        protected virtual void OnContentAdded(DockContentEventArgs e)
        {
            EventHandler<DockContentEventArgs> handler = (EventHandler<DockContentEventArgs>)Events[ContentAddedEvent];
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnContentRemoved(DockContentEventArgs e)
        {
            EventHandler<DockContentEventArgs> handler = (EventHandler<DockContentEventArgs>)Events[ContentRemovedEvent];
            if (handler != null)
                handler(this, e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            SuspendLayout(true);

            AutoHideStripControl.Bounds = ClientRectangle;

            CalculateDockPadding();

            DockWindows[DockState.DockLeft].Width = GetDockWindowSize(DockState.DockLeft);
            DockWindows[DockState.DockRight].Width = GetDockWindowSize(DockState.DockRight);
            DockWindows[DockState.DockTop].Height = GetDockWindowSize(DockState.DockTop);
            DockWindows[DockState.DockBottom].Height = GetDockWindowSize(DockState.DockBottom);

            AutoHideWindow.Bounds = GetAutoHideWindowBounds(AutoHideWindowRectangle);

            DockWindows[DockState.Document].BringToFront();
            AutoHideWindow.BringToFront();

            base.OnLayout(levent);

            if (DocumentStyle == DocumentStyle.SystemMdi && MdiClientExists)
            {
                SetMdiClientBounds(SystemMdiClientBounds);
                InvalidateWindowRegion();
            }
            else if (DocumentStyle == DocumentStyle.DockingMdi)
                InvalidateWindowRegion();

            ResumeLayout(true, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (DockBackColor == BackColor) return;

            Graphics g = e.Graphics;
            SolidBrush bgBrush = new SolidBrush(DockBackColor);
            g.FillRectangle(bgBrush, ClientRectangle);
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            foreach (FloatWindow floatWindow in FloatWindows)
            {
                if (floatWindow.RightToLeft != RightToLeft)
                    floatWindow.RightToLeft = RightToLeft;
            }
        }

        internal AutoHideStripBase AutoHideStripControl
		{
			get
			{	
				if (autoHideStripControl == null)
				{
					autoHideStripControl = AutoHideStripFactory.CreateAutoHideStrip(this);
					Controls.Add(autoHideStripControl);
				}
				return autoHideStripControl;
			}
		}

        internal DockPanelExtender.IAutoHideStripFactory AutoHideStripFactory
        {
            get	{	return Extender.AutoHideStripFactory;	}
        }

        internal Rectangle DockArea
        {
            get
            {
                return new Rectangle(DockPadding.Left, DockPadding.Top,
                                     ClientRectangle.Width - DockPadding.Left - DockPadding.Right,
                                     ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom);
            }
        }

        internal DockPanelExtender.IDockPaneCaptionFactory DockPaneCaptionFactory
        {
            get	{	return Extender.DockPaneCaptionFactory;	}
        }

        internal DockPanelExtender.IDockPaneStripFactory DockPaneStripFactory
        {
            get	{	return Extender.DockPaneStripFactory;	}
        }

        internal IDockContent DummyContent
        {
            get { return dummyContent; }
        }

        internal void AddContent(IDockContent content)
        {
            if (content == null)
                throw(new ArgumentNullException());

            if (!Contents.Contains(content))
            {
                Contents.Add(content);
                OnContentAdded(new DockContentEventArgs(content));
            }
        }

        internal void AddFloatWindow(FloatWindow floatWindow)
        {
            if (FloatWindows.Contains(floatWindow))
                return;

            FloatWindows.Add(floatWindow);
        }

        internal void AddPane(DockPane pane)
        {
            if (Panes.Contains(pane))
                return;

            Panes.Add(pane);
        }

        internal Rectangle GetTabStripRectangle(DockState dockState)
        {
            return AutoHideStripControl.GetTabStripRectangle(dockState);
        }

        internal void RemoveContent(IDockContent content)
        {
            if (content == null)
                throw(new ArgumentNullException());
			
            if (Contents.Contains(content))
            {
                Contents.Remove(content);
                OnContentRemoved(new DockContentEventArgs(content));
            }
        }

        internal void RemovePane(DockPane pane)
        {
            if (!Panes.Contains(pane))
                return;

            Panes.Remove(pane);
        }

        internal void ResetAutoHideStripControl()
        {
            if (autoHideStripControl != null)
                autoHideStripControl.Dispose();

            autoHideStripControl = null;
        }

        private Control DummyControl
        {
            get	{	return dummyControl;	}
        }

        private int GetDockWindowSize(DockState dockState)
        {
            if (dockState == DockState.DockLeft || dockState == DockState.DockRight)
            {
                int width = ClientRectangle.Width - DockPadding.Left - DockPadding.Right;
                int dockLeftSize = m_dockLeftPortion >= 1 ? (int)m_dockLeftPortion : (int)(width * m_dockLeftPortion);
                int dockRightSize = m_dockRightPortion >= 1 ? (int)m_dockRightPortion : (int)(width * m_dockRightPortion);

                if (dockLeftSize < MeasurePane.MinSize)
                    dockLeftSize = MeasurePane.MinSize;
                if (dockRightSize < MeasurePane.MinSize)
                    dockRightSize = MeasurePane.MinSize;

                if (dockLeftSize + dockRightSize > width - MeasurePane.MinSize)
                {
                    int adjust = (dockLeftSize + dockRightSize) - (width - MeasurePane.MinSize);
                    dockLeftSize -= adjust / 2;
                    dockRightSize -= adjust / 2;
                }

                return dockState == DockState.DockLeft ? dockLeftSize : dockRightSize;
            }

            if (dockState == DockState.DockTop || dockState == DockState.DockBottom)
            {
                int height = ClientRectangle.Height - DockPadding.Top - DockPadding.Bottom;
                int dockTopSize = m_dockTopPortion >= 1 ? (int)m_dockTopPortion : (int)(height * m_dockTopPortion);
                int dockBottomSize = m_dockBottomPortion >= 1 ? (int)m_dockBottomPortion : (int)(height * m_dockBottomPortion);

                if (dockTopSize < MeasurePane.MinSize)
                    dockTopSize = MeasurePane.MinSize;
                if (dockBottomSize < MeasurePane.MinSize)
                    dockBottomSize = MeasurePane.MinSize;

                if (dockTopSize + dockBottomSize > height - MeasurePane.MinSize)
                {
                    int adjust = (dockTopSize + dockBottomSize) - (height - MeasurePane.MinSize);
                    dockTopSize -= adjust / 2;
                    dockBottomSize -= adjust / 2;
                }

                return dockState == DockState.DockTop ? dockTopSize : dockBottomSize;
            }

            return 0;
        }

        private void MdiClientHandleAssigned(object sender, EventArgs e)
		{
			SetMdiClient();
			PerformLayout();
		}

        private void MdiClient_Layout(object sender, LayoutEventArgs e)
		{
			if (DocumentStyle != DocumentStyle.DockingMdi)
				return;

			foreach (DockPane pane in Panes)
				if (pane.DockState == DockState.Document)
					pane.SetContentBounds();

			InvalidateWindowRegion();
		}

        //private Rectangle DocumentRectangle
        //{
        //    get
        //    {
        //        Rectangle rect = DockArea;
        //        if (DockWindows[DockState.DockLeft].VisibleNestedPanes.Count != 0)
        //        {
        //            rect.X += (int)(DockArea.Width * DockLeftPortion);
        //            rect.Width -= (int)(DockArea.Width * DockLeftPortion);
        //        }
        //        if (DockWindows[DockState.DockRight].VisibleNestedPanes.Count != 0)
        //            rect.Width -= (int)(DockArea.Width * DockRightPortion);
        //        if (DockWindows[DockState.DockTop].VisibleNestedPanes.Count != 0)
        //        {
        //            rect.Y += (int)(DockArea.Height * DockTopPortion);
        //            rect.Height -= (int)(DockArea.Height * DockTopPortion);
        //        }
        //        if (DockWindows[DockState.DockBottom].VisibleNestedPanes.Count != 0)
        //            rect.Height -= (int)(DockArea.Height * DockBottomPortion);

        //        return rect;
        //    }
        //}

        private bool ShouldSerializeDefaultFloatWindowSize()
        {
            return DefaultFloatWindowSize != new Size(300, 300);
        }

        private void CalculateDockPadding()
		{
			DockPadding.All = 0;

			int height = AutoHideStripControl.MeasureHeight();

			if (AutoHideStripControl.GetNumberOfPanes(DockState.DockLeftAutoHide) > 0)
				DockPadding.Left = height;
			if (AutoHideStripControl.GetNumberOfPanes(DockState.DockRightAutoHide) > 0)
				DockPadding.Right = height;
			if (AutoHideStripControl.GetNumberOfPanes(DockState.DockTopAutoHide) > 0)
				DockPadding.Top = height;
			if (AutoHideStripControl.GetNumberOfPanes(DockState.DockBottomAutoHide) > 0)
				DockPadding.Bottom = height;
		}

        internal void RemoveFloatWindow(FloatWindow floatWindow)
		{
			if (!FloatWindows.Contains(floatWindow))
				return;

			FloatWindows.Remove(floatWindow);
		}

        public void SuspendLayout(bool allWindows)
		{
            FocusManager.SuspendFocusTracking();
			SuspendLayout();
			if (allWindows)
				SuspendMdiClientLayout();
		}

        public void ResumeLayout(bool performLayout, bool allWindows)
		{
            FocusManager.ResumeFocusTracking();
            ResumeLayout(performLayout);
            if (allWindows)
                ResumeMdiClientLayout(performLayout);
		}

        internal Form ParentForm
		{
			get
			{	
				if (!IsParentFormValid())
					throw new InvalidOperationException(Strings.DockPanel_ParentForm_Invalid);

				return GetMdiClientController().ParentForm;
			}
		}

        private bool IsParentFormValid()
		{
			if (DocumentStyle == DocumentStyle.DockingSdi || DocumentStyle == DocumentStyle.DockingWindow)
				return true;

            if (!MdiClientExists)
                GetMdiClientController().RenewMdiClient();

            return (MdiClientExists);
		}

        protected override void OnParentChanged(EventArgs e)
		{
            SetAutoHideWindowParent();
            GetMdiClientController().ParentForm = (Parent as Form);
			base.OnParentChanged (e);
		}

        private void SetAutoHideWindowParent()
        {
            Control parent;
            if (DocumentStyle == DocumentStyle.DockingMdi ||
                DocumentStyle == DocumentStyle.SystemMdi)
                parent = Parent;
            else
                parent = this;

            if (AutoHideWindow.Parent != parent)
            {
                AutoHideWindow.Parent = parent;
                AutoHideWindow.BringToFront();
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged (e);

			if (Visible)
				SetMdiClient();
		}

        private Rectangle SystemMdiClientBounds
		{
			get
			{
				if (!IsParentFormValid() || !Visible)
					return Rectangle.Empty;

				Rectangle rect = ParentForm.RectangleToClient(RectangleToScreen(DocumentWindowBounds));
				return rect;
			}
		}

        internal Rectangle DocumentWindowBounds
		{
			get
			{
				Rectangle rectDocumentBounds = DisplayRectangle;
				if (DockWindows[DockState.DockLeft].Visible)
				{
					rectDocumentBounds.X += DockWindows[DockState.DockLeft].Width;
					rectDocumentBounds.Width -= DockWindows[DockState.DockLeft].Width;
				}
				if (DockWindows[DockState.DockRight].Visible)
					rectDocumentBounds.Width -= DockWindows[DockState.DockRight].Width;
				if (DockWindows[DockState.DockTop].Visible)
				{
					rectDocumentBounds.Y += DockWindows[DockState.DockTop].Height;
					rectDocumentBounds.Height -= DockWindows[DockState.DockTop].Height;
				}
				if (DockWindows[DockState.DockBottom].Visible)
					rectDocumentBounds.Height -= DockWindows[DockState.DockBottom].Height;

				return rectDocumentBounds;

			}
		}

        private void InvalidateWindowRegion()
        {
            if (DesignMode)
                return;

            if (m_dummyControlPaintEventHandler == null)
                m_dummyControlPaintEventHandler = new PaintEventHandler(DummyControl_Paint);

            DummyControl.Paint += m_dummyControlPaintEventHandler;
            DummyControl.Invalidate();
        }

        void DummyControl_Paint(object sender, PaintEventArgs e)
        {
            DummyControl.Paint -= m_dummyControlPaintEventHandler;
            UpdateWindowRegion();
        }

        private void UpdateWindowRegion()
		{
			if (DocumentStyle == DocumentStyle.DockingMdi)
				UpdateWindowRegion_ClipContent();
			else if (DocumentStyle == DocumentStyle.DockingSdi || DocumentStyle == DocumentStyle.DockingWindow)
				UpdateWindowRegion_FullDocumentArea();
			else if (DocumentStyle == DocumentStyle.SystemMdi)
				UpdateWindowRegion_EmptyDocumentArea();
		}

        private void UpdateWindowRegion_FullDocumentArea()
		{
			SetRegion(null);
		}

        private void UpdateWindowRegion_EmptyDocumentArea()
		{
			Rectangle rect = DocumentWindowBounds;
			SetRegion(new[] { rect });
		}

        private void UpdateWindowRegion_ClipContent()
		{
			int count = 0;
			foreach (DockPane pane in Panes)
			{
				if (!pane.Visible || pane.DockState != DockState.Document)
					continue;

				count ++;
			}

            if (count == 0)
            {
                SetRegion(null);
                return;
            }

			Rectangle[] rects = new Rectangle[count];
			int i = 0;
			foreach (DockPane pane in Panes)
			{
				if (!pane.Visible || pane.DockState != DockState.Document)
					continue;

                rects[i] = RectangleToClient(pane.RectangleToScreen(pane.ContentRectangle));
				i++;
			}

			SetRegion(rects);
		}

        private void SetRegion(Rectangle[] clipRects)
		{
			if (!IsClipRectsChanged(clipRects))
				return;

			m_clipRects = clipRects;

			if (m_clipRects == null || m_clipRects.GetLength(0) == 0)
				Region = null;
			else
			{
				Region region = new Region(new Rectangle(0, 0, Width, Height));
				foreach (Rectangle rect in m_clipRects)
					region.Exclude(rect);
				Region = region;
			}
		}

        private bool IsClipRectsChanged(Rectangle[] clipRects)
		{
			if (clipRects == null && m_clipRects == null)
				return false;
		    if ((clipRects == null) != (m_clipRects == null))
		        return true;

		    foreach (Rectangle rect in clipRects)
			{
				bool matched = false;
				foreach (Rectangle rect2 in m_clipRects)
				{
					if (rect == rect2)
					{
						matched = true;
						break;
					}
				}
				if (!matched)
					return true;
			}

			foreach (Rectangle rect2 in m_clipRects)
			{
				bool matched = false;
				foreach (Rectangle rect in clipRects)
				{
					if (rect == rect2)
					{
						matched = true;
						break;
					}
				}
				if (!matched)
					return true;
			}
			return false;
		}

        private FocusManagerImpl focusManager;
        private DockPanelExtender extender;
        private DockPaneCollection panes;
        private FloatWindowCollection floatWindows;
        private AutoHideWindowControl autoHideWindow;
        private DockWindowCollection dockWindows;
        private IDockContent dummyContent;
        private Control dummyControl;
        private Color backColor;
        private AutoHideStripBase autoHideStripControl;
        private bool disposed;
        private bool m_allowEndUserDocking = true;
        private bool m_allowEndUserNestedDocking = true;
        private DockContentCollection m_contents = new DockContentCollection();
        private bool m_rightToLeftLayout;
        private bool m_showDocumentIcon;
        private DockPanelSkin m_dockPanelSkin = new DockPanelSkin();
        private DocumentTabStripLocation m_documentTabStripLocation = DocumentTabStripLocation.Top;
        private double m_dockBottomPortion = 0.25;
        private double m_dockLeftPortion = 0.25;
        private double m_dockRightPortion = 0.25;
        private double m_dockTopPortion = 0.25;
        private Size m_defaultFloatWindowSize = new Size(300, 300);
        private DocumentStyle m_documentStyle = DocumentStyle.DockingMdi;
        private PaintEventHandler m_dummyControlPaintEventHandler;
        private Rectangle[] m_clipRects;
        private static readonly object ContentAddedEvent = new object();
        private static readonly object ContentRemovedEvent = new object();
    }
}

// To simplify the process of finding the toolbox bitmap resource:
// #1 Create an internal class called "resfinder" outside of the root namespace.
// #2 Use "resfinder" in the toolbox bitmap attribute instead of the control name.
// #3 use the "<default namespace>.<resourcename>" string to locate the resource.
// See: http://www.bobpowell.net/toolboxbitmap.htm
internal class resfinder
{
}