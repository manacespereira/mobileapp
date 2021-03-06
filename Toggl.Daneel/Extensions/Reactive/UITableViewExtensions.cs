﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class UITableViewExtensions
    {
        public static IDisposable Bind<TModel, TCell>(this IReactive<UITableView> tableView, ReactiveSectionedListTableViewSource<TModel, TCell> dataSource)
            where TCell : BaseTableViewCell<TModel>
            => new ReactiveTableViewBinder<TModel, TCell>(tableView.Base, dataSource);

        public static IObserver<IImmutableList<IImmutableList<TModel>>> Sections<TModel>(
            this IReactive<UITableView> reactive, ReloadTableViewSource<TModel> dataSource)
        {
            return Observer.Create<IImmutableList<IImmutableList<TModel>>>(list =>
            {
                dataSource.SetItems(list);
                reactive.Base.ReloadData();
            });
        }

        public static IObserver<IEnumerable<TModel>> Items<TModel>(
            this IReactive<UITableView> reactive, ReloadTableViewSource<TModel> dataSource)
        {
            return Observer.Create<IEnumerable<TModel>>(list =>
            {
                dataSource.SetItems(list.ToImmutableList());
                reactive.Base.ReloadData();
            });
        }
    }
}
