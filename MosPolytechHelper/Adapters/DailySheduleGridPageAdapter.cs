﻿namespace MosPolyHelper.Adapters
{
    using Android.Views;
    using Android.Widget;
    using AndroidX.RecyclerView.Widget;
    using AndroidX.ViewPager.Widget;
    using Java.Lang;
    using MosPolyHelper.Domains.ScheduleDomain;
    using System;
    using Object = Java.Lang.Object;

    public class DailySheduleGridPageAdapter : PagerAdapter
    {
        View view;
        DailyShceduleGridAdapter recyclerAdapter;
        RecyclerView recyclerView;
        public Schedule Schedule;
        Schedule.Filter scheduleFilter;
        bool showEmptyLessons;
        bool showColoredLessons;

        public event Action<DateTime> ItemClick;

        public DailySheduleGridPageAdapter(Schedule schedule, Schedule.Filter scheduleFilter, 
            bool showEmptyLessons, bool showColoredLessons)
        {
            this.scheduleFilter = scheduleFilter;
            this.Schedule = schedule;
            this.showEmptyLessons = showEmptyLessons;
            this.showColoredLessons = showColoredLessons;
        }

        public void BuildSchedule(Schedule schedule, Schedule.Filter scheduleFilter, bool showEmptyLessons, bool showColoredLessons)
        {
            this.scheduleFilter = scheduleFilter;
            this.Schedule = schedule;
            this.showEmptyLessons = showEmptyLessons;
            this.showColoredLessons = showColoredLessons;
            this.recyclerAdapter?.BuildSchedule(schedule, scheduleFilter, showEmptyLessons, showColoredLessons);
        }


        public override int Count => 1;
        public DateTime FirstPosDate => this.recyclerAdapter?.FirstPosDate ?? DateTime.Today;

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(string.Empty);
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            if (this.view == null)
            {
                var inflater = LayoutInflater.From(container.Context);
                this.view = (ViewGroup)inflater.Inflate(Resource.Layout.page_schedule, container, false);
                container.AddView(this.view);
            }
            if (this.Schedule == null)
            {
                return this.view;
            }
            if (this.recyclerView == null)
            {
                this.recyclerView = this.view.FindViewById<RecyclerView>(Resource.Id.recycler_schedule);
                this.view.FindViewById<TextView>(Resource.Id.text_day).Visibility = ViewStates.Gone;
            }
            if (this.recyclerAdapter == null)
            {
                this.recyclerAdapter = new DailyShceduleGridAdapter(
                       this.view.FindViewById<TextView>(Resource.Id.text_null_lesson),
                       this.Schedule, scheduleFilter, this.showEmptyLessons, this.showColoredLessons);
                this.recyclerAdapter.ItemClick += date => ItemClick?.Invoke(date);
                this.recyclerView.SetItemAnimator(null);
                this.recyclerView.SetLayoutManager(new GridLayoutManager(container.Context, 3));
                this.recyclerView.SetAdapter(this.recyclerAdapter);
                float scale = this.recyclerView.Context.Resources.DisplayMetrics.Density;
                int dp8InPx = (int)(4 * scale + 0.5f);
                this.recyclerView.AddItemDecoration(new DailyShceduleGridAdapter.ItemDecoration(dp8InPx));
            }
            else
            {
                this.recyclerAdapter.BuildSchedule(this.Schedule, scheduleFilter, this.showEmptyLessons, this.showColoredLessons);
            }
            this.recyclerView.ScrollToPosition((DateTime.Today - this.recyclerAdapter.FirstPosDate).Days);
            return this.view;
        }

        public void GoHome()
        {
            if (this.recyclerAdapter != null)
            {
                this.CurrentDate = DateTime.Today;
            }
        }

        public override void DestroyItem(ViewGroup container, int position, Object @object)
        {
        }

        public override bool IsViewFromObject(View view, Object @object)
        {
            return view == @object;
        }

        public DateTime CurrentDate
        {
            get
            {
                if (this.recyclerView.GetLayoutManager() is GridLayoutManager grid)
                {
                    return this.recyclerAdapter.FirstPosDate.AddDays(grid.FindFirstCompletelyVisibleItemPosition());
                }
                else
                {
                    return DateTime.Today;
                }
            }
            set
            {
                var day = (value - this.recyclerAdapter.FirstPosDate).Days;
                this.recyclerView?.ScrollToPosition(day < 0 ? 0 : day);
            }
        }
    }
}