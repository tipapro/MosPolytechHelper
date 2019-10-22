﻿namespace MosPolytechHelper.Features.Common
{
    abstract class FragmentBase : Android.Support.V4.App.Fragment
    {
        public Fragments FragmentType { get; }

        public FragmentBase(Fragments fragmentType)
        {
            this.FragmentType = fragmentType;
        }
    }

    public enum Fragments
    {
        ScheduleMain,
        ScheduleManager,
        Other
    }
}