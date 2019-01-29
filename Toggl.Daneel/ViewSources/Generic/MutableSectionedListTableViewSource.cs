using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Collections.Changes;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public abstract class MutableSectionedListTableViewSource<TModel, TCell> : UITableViewSource
        where TCell : BaseTableViewCell<TModel>
    {
        private readonly string cellIdentifier;

        private readonly IReadOnlyList<IReadOnlyList<TModel>> items;

        public ISubject<TModel> ItemSelected { get; } = new Subject<TModel>();

        protected IReadOnlyList<IReadOnlyList<TModel>> DisplayedItems
            => displayedItems.Select(section => section.AsReadOnly()).ToList().AsReadOnly();

        private List<List<TModel>> displayedItems;

        protected MutableSectionedListTableViewSource(IReadOnlyList<IReadOnlyList<TModel>> items, string cellIdentifier)
        {
            this.items = items;
            this.cellIdentifier = cellIdentifier;

            reloadDisplayedData();
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = (TCell)tableView.DequeueReusableCell(cellIdentifier, indexPath);
            cell.Item = displayedItems[indexPath.Section][indexPath.Row];
            return cell;
        }

        public override nint NumberOfSections(UITableView tableView)
            => displayedItems.Count;

        public override nint RowsInSection(UITableView tableview, nint section)
            => displayedItems[(int)section].Count;

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = displayedItems[indexPath.Section][indexPath.Row];
            ItemSelected.OnNext(item);
        }

        public void ChangeDisplayedCollection(ICollectionChange change)
        {
            switch (change)
            {
                case InsertSectionCollectionChange<TModel> insert:
                    insertSection(insert.Index, insert.Item);
                    break;

                case AddRowCollectionChange<TModel> addRow:
                    add(addRow.Index, addRow.Item);
                    break;

                case RemoveRowCollectionChange removeRow:
                    remove(removeRow.Index);
                    break;

                case MoveRowToNewSectionCollectionChange<TModel> moveRowToNewSection:
                    remove(moveRowToNewSection.OldIndex);
                    insertSection(moveRowToNewSection.Index, moveRowToNewSection.Item);
                    break;

                case MoveRowWithinExistingSectionsCollectionChange<TModel> moveRow:
                    remove(moveRow.OldIndex);
                    add(moveRow.Index, moveRow.Item);
                    break;

                case UpdateRowCollectionChange<TModel> updateRow:
                    update(updateRow.Index, updateRow.Item);
                    break;

                case ReloadCollectionChange _:
                    reloadDisplayedData();
                    break;

                case AddMultipleRowsCollectionChange<TModel> addMultipleRowsChange:
                    addMultipleRows(addMultipleRowsChange.AddedRowChanges);
                    break;

                case RemoveMultipleRowsCollectionChange removeMultipleRowsCollectionChange:
                    removeMultipleRows(removeMultipleRowsCollectionChange.RemovedIndexes);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void removeMultipleRows(IEnumerable<SectionedIndex> removedRows)
        {
            foreach (var removedRow in removedRows)
            {
                var section = displayedItems[removedRow.Section];
                section.RemoveAt(removedRow.Row);
                if (section.Count == 0)
                    displayedItems.Remove(section);
            }
        }

        private void addMultipleRows(IEnumerable<AddRowCollectionChange<TModel>> addedRows)
        {
            foreach (var addedRow in addedRows)
                add(addedRow.Index, addedRow.Item);
        }

        private void insertSection(int index, TModel item)
        {
            displayedItems.Insert(index, new List<TModel> { item });
        }

        private void add(SectionedIndex index, TModel item)
        {
            displayedItems[index.Section].Insert(index.Row, item);
        }

        private void remove(SectionedIndex index)
        {
            if (displayedItems[index.Section].Count == 1)
            {
                displayedItems.RemoveAt(index.Section);
            }
            else
            {
                displayedItems[index.Section].RemoveAt(index.Row);
            }
        }

        private void update(SectionedIndex index, TModel item)
        {
            displayedItems[index.Section][index.Row] = item;
        }

        private void reloadDisplayedData()
        {
            displayedItems = new List<List<TModel>>(items.Select(list => new List<TModel>(list)));
        }
    }
}
