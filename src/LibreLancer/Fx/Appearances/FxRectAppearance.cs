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
using LibreLancer.Utf.Ale;
namespace LibreLancer.Fx
{
	public class FxRectAppearance : FxBasicAppearance
	{
		public bool CenterOnPos;
		public bool ViewingAngleFade;
		public AlchemyFloatAnimation Scale;
		public AlchemyFloatAnimation Length;
		public AlchemyFloatAnimation Width;

		public FxRectAppearance (AlchemyNode ale) : base(ale)
		{
			AleParameter temp;
			if (ale.TryGetParameter("RectApp_CenterOnPos", out temp))
			{
				CenterOnPos = (bool)temp.Value;
			}
			if (ale.TryGetParameter("RectApp_ViewingAngleFade", out temp))
			{
				ViewingAngleFade = (bool)temp.Value;
			}
			if (ale.TryGetParameter("RectApp_Scale", out temp))
			{
				Scale = (AlchemyFloatAnimation)temp.Value;
			}
			if (ale.TryGetParameter("RectApp_Length", out temp))
			{
				Length = (AlchemyFloatAnimation)temp.Value;
			}
			if (ale.TryGetParameter("RectApp_Width", out temp))
			{
				Width = (AlchemyFloatAnimation)temp.Value;
			}
		}

		Vector3 Project(Billboards billboards, Vector3 pt)
		{
			var mvp = billboards.Camera.ViewProjection;
			var pt4 = (mvp * new Vector4(pt, 1));
			pt4 /= pt4.W;
			return pt4.Xyz;
		}

		public override void Draw(ref Particle particle, float lasttime, float globaltime, NodeReference reference, ResourceManager res, Billboards billboards, ref Matrix4 transform, float sparam)
		{
			var time = particle.TimeAlive / particle.LifeSpan;
            var node_tr = GetAttachment(reference, transform);
			var src_pos = particle.Position;
			var l = Length.GetValue(sparam, time);
			var w = Width.GetValue(sparam, time);
			var sc = Scale.GetValue(sparam, time);
			if (!CenterOnPos) {
				var nd = particle.Normal.Normalized();
				src_pos += nd * (l * sc * 0.25f);
			}
			var p = node_tr.Transform(src_pos);
			Texture2D tex;
			Vector2 tl, tr, bl, br;
			HandleTexture(res, globaltime, sparam, ref particle, out tex, out tl, out tr, out bl, out br);
			var c = Color.GetValue(sparam, time);
			var a = Alpha.GetValue(sparam, time);
			var p2 = node_tr.Transform(src_pos + (particle.Normal  * 20));
            //var n = (p2 - p).Normalized();
            var n = (transform * new Vector4(particle.Normal.Normalized(), 0)).Xyz.Normalized();
			billboards.DrawRectAppearance(
				tex,
				p,
				new Vector2(l, w) * sc * 0.5f,
				new Color4(c,a),
				tl,
				tr,
				bl,
				br,
				n,
                Rotate == null ? 0f : MathHelper.DegreesToRadians(Rotate.GetValue(sparam, time)),
				SortLayers.OBJECT,
				BlendInfo
			);
			if (DrawNormals)
			{
				Debug.DrawLine(p - (n * 12), p + (n * 12));
			}
		}
	}
}

