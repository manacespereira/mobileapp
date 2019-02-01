﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using Toggl.Foundation.Exceptions;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.ViewModels.Selectable;
using Toggl.Foundation.Services;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Settings;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    public abstract class SelectUserCalendarsViewModelBase : MvxViewModel<bool, string[]>
    {
        private readonly IUserPreferences userPreferences;

        public IObservable<IImmutableList<CollectionSection<string, SelectableUserCalendarViewModel>>> Calendars { get; }

        public InputAction<SelectableUserCalendarViewModel> SelectCalendar { get; }

        protected bool ForceItemSelection { get; private set; }

        protected HashSet<string> SelectedCalendarIds { get; } = new HashSet<string>();

        protected SelectUserCalendarsViewModelBase(
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.userPreferences = userPreferences;

            SelectCalendar = rxActionFactory.FromAction<SelectableUserCalendarViewModel>(toggleCalendarSelection);

            Calendars = interactorFactory
                .GetUserCalendars()
                .Execute()
                .Catch((NotAuthorizedException _) => Observable.Return(new List<UserCalendar>()))
                .Select(group);
        }

        public sealed override void Prepare(bool parameter)
        {
            ForceItemSelection = parameter;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            SelectedCalendarIds.AddRange(userPreferences.EnabledCalendarIds());
        }

        private IImmutableList<CollectionSection<string, SelectableUserCalendarViewModel>> group(IEnumerable<UserCalendar> calendars)
            => calendars
                .Select(toSelectable)
                .GroupBy(calendar => calendar.SourceName)
                .Select(group =>
                    new CollectionSection<string, SelectableUserCalendarViewModel>(
                        group.First().SourceName,
                        group.OrderBy(calendar => calendar.Name)
                    )
                )
                .ToImmutableList();

        private SelectableUserCalendarViewModel toSelectable(UserCalendar calendar)
            => new SelectableUserCalendarViewModel(calendar, SelectedCalendarIds.Contains(calendar.Id));

        private void toggleCalendarSelection(SelectableUserCalendarViewModel calendar)
        {
            if (SelectedCalendarIds.Contains(calendar.Id))
                SelectedCalendarIds.Remove(calendar.Id);
            else
                SelectedCalendarIds.Add(calendar.Id);
        }
    }
}
