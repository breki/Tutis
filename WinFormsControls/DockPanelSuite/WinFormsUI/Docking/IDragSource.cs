using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal interface IDragSource
    {
        Control DragControl { get; }
    }
}