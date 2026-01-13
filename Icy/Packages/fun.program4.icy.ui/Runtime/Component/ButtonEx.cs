/*
 * Copyright 2025-2026 @ProgramForFun. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using Icy.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Icy.UI
{
	/// <summary>
	/// 扩展Button
	/// </summary>
	public class ButtonEx : Button
	{
		/// <summary>
		/// 是否是灰化状态
		/// </summary>
		public bool IsGray { get; protected set; }
		/// <summary>
		/// ImageEx正在显示的Sprite；
		/// 如果这个按钮没有对应的Image，返回null
		/// </summary>
		public Sprite Sprite => _ImageEx == null ? null : _ImageEx.sprite;
		/// <summary>
		/// 按钮本体Sprite
		/// </summary>
		protected ImageEx _ImageEx;


		protected override void Start()
		{
			base.Start();
			_ImageEx = GetComponentInChildren<ImageEx>();
		}

		/// <summary>
		/// 设置按钮的Sprite
		/// </summary>
		public void SetSprite(string sprite)
		{
			if (_ImageEx == null)
				Log.Error($"Call {nameof(SetSprite)} to a {nameof(ButtonEx)} without Image", nameof(ButtonEx));
			else
				_ImageEx.SetSprite(sprite);
		}

		internal void SetSprite(Sprite sprite)
		{
			_ImageEx.sprite = sprite;
		}

		/// <summary>
		/// 设置按钮是否灰化
		/// </summary>
		public void SetGray(bool gray)
		{
			// TODO
		}

		// TODO
	}
}
