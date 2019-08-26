﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
namespace LibreLancer.ImUI
{
	public abstract class DockTab : IDisposable
	{
        public string DocumentName = "document";
        string _title = "tab";

        public string Title
        {
            get { return _title;  }
            set { _title = value; UpdateRenderTitle(); }
        }
        public string RenderTitle { get; private set; }

        static long _ids = 1;
		static Random rand = new Random();
		protected long Unique { get; private set; }

        void UpdateRenderTitle()
        {
            RenderTitle = ImGuiExt.IDWithExtra(_title, Unique.ToString());
        }

        protected DockTab()
		{
			Unique = _ids;
			_ids += 2;
            UpdateRenderTitle();
		}
		public abstract void Draw();
		public virtual void Update(double elapsed)
		{
		}
		public virtual void Dispose()
		{
		}
	}
}
