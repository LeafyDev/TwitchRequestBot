// ---------------------------------------------------------
// Copyrights (c) 2014-2018 LeafyDev 🍂 All rights reserved.
// ---------------------------------------------------------

using System.Drawing;
using System.Windows.Forms;

namespace RequestBotThing
{
    /// <summary>
    ///     A ListView with DragDrop reordering.
    ///     <see cref="http://support.microsoft.com/kb/822483/en-us" />
    /// </summary>
    public class ListViewWithReordering : ListView
    {
        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);

            //Begins a drag-and-drop operation in the ListView control.
            DoDragDrop(SelectedItems, DragDropEffects.Move);
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            var len = drgevent.Data.GetFormats().Length - 1;

            for (var i = 0; i <= len; i++)
                if (drgevent.Data.GetFormats()[i]
                    .Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                    drgevent.Effect = DragDropEffects.Move;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            //Return if the items are not selected in the ListView control.
            if (SelectedItems.Count == 0) return;

            //Returns the location of the mouse pointer in the ListView control.
            var cp = PointToClient(new Point(drgevent.X, drgevent.Y));

            //Obtain the item that is located at the specified location of the mouse pointer.
            var dragToItem = GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
                return;

            //Obtain the index of the item at the mouse pointer.
            var dragIndex = dragToItem.Index;
            var sel = new ListViewItem[SelectedItems.Count];

            for (var i = 0; i <= SelectedItems.Count - 1; i++)
                sel[i] = SelectedItems[i];

            for (var i = 0; i < sel.GetLength(0); i++)
            {
                //Obtain the ListViewItem to be dragged to the target location.
                var dragItem = sel[i];
                var itemIndex = dragIndex;

                if (itemIndex == dragItem.Index)
                    return;

                if (dragItem.Index < itemIndex)
                    itemIndex++;
                else
                    itemIndex = dragIndex + i;

                //Insert the item at the mouse pointer.
                var insertItem = (ListViewItem) dragItem.Clone();
                Items.Insert(itemIndex, insertItem);

                //Removes the item from the initial location while
                //the item is moved to the new location.
                Items.Remove(dragItem);
            }
        }
    }
}