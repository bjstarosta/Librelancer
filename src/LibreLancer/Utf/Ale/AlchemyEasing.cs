﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;

namespace LibreLancer.Utf.Ale
{
	public enum EasingTypes : byte
	{
		Linear = 1,
		EaseIn = 2,
		EaseOut = 3,
		EaseInOut = 4,
		Step = 5
	}
	public static class AlchemyEasing
	{
		public static Color3f EaseColor(EasingTypes type, float time, float t1, float t2, Color3f c1, Color3f c2)
		{
			var h1 = HSLColor.FromRGB (c1);
			var h2 = HSLColor.FromRGB (c2);

			float h = Ease (type, time, t1, t2, h1.H, h2.H);
			float s = Ease (type, time, t1, t2, h1.S, h2.S);
			float l = Ease (type, time, t1, t2, h1.L, h2.L);

			return new HSLColor (h, s, l).ToRGB ();
		}

		public static float Ease(EasingTypes type, float time, float t1, float t2, float v1, float v2)
		{
			switch (type) {
			case EasingTypes.Linear:
				return Linear (time, t1, t2, v1, v2);
			case EasingTypes.EaseIn:
				return EaseIn (time, t1, t2, v1, v2);
			case EasingTypes.EaseOut:
				return EaseOut (time, t1, t2, v1, v2);
			case EasingTypes.EaseInOut:
				return EaseInOut (time, t1, t2, v1, v2);
			case EasingTypes.Step:
				return Step(time,t1,t2,v1,v2);
			}
			throw new InvalidOperationException ();
		}

		static float Linear(float time, float t1, float t2, float v1, float v2)
		{
			var time_pct = (time - t1) / t2;
			return v1 + (v2 - v1) * time_pct;
		}

		static float EaseIn(float time, float t1, float t2, float v1, float v2)
		{
			var x = (time - t1) / t2;
			// very close approximation to cubic-bezier(0.42, 0, 1.0, 1.0)
			var y = (float)Math.Pow(x, 1.685);
			return v1 + (v2 - v1) * y;
		}

		static float EaseOut(float time, float t1, float t2, float v1, float v2)
		{
			var x = (time - t1) / t2;
			// very close approximation to cubic-bezier(0, 0, 0.58, 1.0)
			var y = 1f - (float)Math.Pow (1 - x, 1.685);
			return v1 + (v2 - v1) * y;
		}

		//0.5 * (0.5 ^ 1.925)
		const double EASE_IN_OUT_CONST = 0.131670129494354;

		static float EaseInOut(float time, float t1, float t2, float v1, float v2)
		{
			// very close approximation to cubic-bezier(0.42, 0, 0.58, 1.0)
			var x = (time - t1) / t2;
			float y;
			if (x < 0.5f) {
				y = (float)(EASE_IN_OUT_CONST * Math.Pow (x, 1.925));
			} else {
				y = (float)(1 - EASE_IN_OUT_CONST * Math.Pow(1-x, 1.925));
			}
			return v1 + (v2 - v1) * y;
		}

		static float Step(float time, float t1, float t2, float v1, float v2)
		{
			return v1;
		}
	}
}

