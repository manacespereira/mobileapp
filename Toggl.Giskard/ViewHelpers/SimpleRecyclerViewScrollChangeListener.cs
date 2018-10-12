using System;
using System.Reactive;
using System.Reactive.Subjects;
using Android.Support.V7.Widget;

namespace Toggl.Giskard.ViewHelpers
{
    public sealed class SimpleRecyclerViewScrollChangeListener : RecyclerView.OnScrollListener
    {
        private readonly ISubject<Unit> onScroll;

        public SimpleRecyclerViewScrollChangeListener(ISubject<Unit> onScrollSubject)
        {
            onScroll = onScrollSubject;
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            onScroll?.OnNext(Unit.Default);
        }
        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            onScroll?.OnNext(Unit.Default);
        }
    }
}
