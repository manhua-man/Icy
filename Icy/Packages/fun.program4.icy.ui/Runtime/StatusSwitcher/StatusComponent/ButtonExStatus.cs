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


using Icy.Asset;
using Icy.Base;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace Icy.UI
{
	/// <summary>
	/// ButtonEx状态，记录灰化、可交互、Sprite
	/// </summary>
	[HideLabel]
	[System.Serializable]
	public class ButtonExStatus : StatusSwitcherStatusBase
	{
		[InlineProperty]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(Record))]
		public bool Gray;

		[InlineProperty]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(Record))]
		public bool Interactable;

		[InlineProperty]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(Record))]
		public string Sprite;


		protected ButtonEx _Btn;

		public override void Init(StatusSwitcherTarget target)
		{
			base.Init(target);
			_Btn = target.GetComponent<ButtonEx>();
		}

		public override void Record()
		{
			Gray = _Btn.IsGray;
			Interactable = _Btn.interactable;

#if UNITY_EDITOR
			if (_Btn.Sprite == null)
				Sprite = string.Empty;
			else
			{
				string path = UnityEditor.AssetDatabase.GetAssetPath(_Btn.Sprite);
				AssetSetting assetSetting = SettingsHelper.GetSettingEditor<AssetSetting>();
				if (AssetManager.IsAddressableInSetting(assetSetting.DefaultPackageName))
					Sprite = Path.GetFileNameWithoutExtension(path);
				else
					Sprite = path;
			}
#endif
		}

		public override void Apply()
		{
			_Btn.SetGray(Gray);
			_Btn.interactable = Interactable;

			if (!string.IsNullOrEmpty(Sprite))
			{
#if UNITY_EDITOR
				if (Application.isPlaying)
					_Btn.SetSprite(Sprite);
				else
				{
					string[] guid = UnityEditor.AssetDatabase.FindAssets($"t:Sprite {Sprite}");
					if (guid.Length > 0)
					{
						string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid[0]);
						Sprite sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
						_Btn.SetSprite(sprite);
					}
				}
#else
				_Btn.SetSprite(Sprite);
#endif
			}
		}

		public override void CopyFrom(StatusSwitcherStatusBase other)
		{
			ButtonExStatus otherStatus = other as ButtonExStatus;
			Gray = otherStatus.Gray;
			Interactable = otherStatus.Interactable;
			Sprite = otherStatus.Sprite;
		}
	}
}
