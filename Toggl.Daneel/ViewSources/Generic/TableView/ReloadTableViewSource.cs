using System.Collections.Immutable;
using Toggl.Foundation.MvvmCross.Collections;

namespace Toggl.Daneel.ViewSources
{
    public class ReloadTableViewSource<THeader, TModel> : BaseTableViewSource<THeader, TModel>
    {
        public ReloadTableViewSource(CellConfiguration<TModel> configureCell, IImmutableList<CollectionSection<THeader, TModel>> sections)
            : base(configureCell, sections)
        {
        }

        public ReloadTableViewSource(CellConfiguration<TModel> configureCell, IImmutableList<TModel> items = null)
            : base(configureCell, items == null ? ImmutableList<CollectionSection<THeader, TModel>>.Empty : collectionFromList(items))
        {
        }

        public void SetItems(IImmutableList<CollectionSection<THeader, TModel>> sections)
        {
            this.Sections = sections;
        }

        public void SetItems(IImmutableList<TModel> items)
        {
            Sections = collectionFromList(items);
        }

        private static ImmutableList<CollectionSection<THeader, TModel>> collectionFromList(IImmutableList<TModel> items)
        {
            return ImmutableList.Create(new CollectionSection<THeader, TModel>(default(THeader), items));
        }
    }
}
