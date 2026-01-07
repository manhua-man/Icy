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
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Icy.UI
{
	/// <summary>
	/// RectTransform状态，记录Anchor、位置、大小、Pivot
	/// </summary>
	[HideLabel]
	[Serializable]
	public class RectTransformStatus : StatusSwitcherStatusBase
	{
		[InlineProperty]
		[SerializeField]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(RecordAnchorMin), "Record")]
		public Vector2 AnchorMin;

		[InlineProperty]
		[SerializeField]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(RecordAnchorMax), "Record")]
		public Vector2 AnchorMax;

		[InlineProperty]
		[SerializeField]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(RecordAnchoredPosition), "Record")]
		public Vector2 AnchoredPosition;

		[InlineProperty]
		[SerializeField]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(RecordSizeDelta), "Record")]
		public Vector2 SizeDelta;

		[InlineProperty]
		[SerializeField]
		[OnValueChanged(nameof(Apply))]
		[InlineButton(nameof(RecordPivot), "Record")]
		public Vector2 Pivot;


		[Button("Anchor")]
		protected void ChangeAnchor()
		{
			ShowAnchorPresetPopup(Target.transform as RectTransform);
		}

		protected void RecordAnchorMin()
		{
			RectTransform rectTrans = Target.transform as RectTransform;
			AnchorMin = rectTrans.anchorMin;
		}

		protected void RecordAnchorMax()
		{
			RectTransform rectTrans = Target.transform as RectTransform;
			AnchorMax = rectTrans.anchorMax;
		}

		protected void RecordAnchoredPosition()
		{
			RectTransform rectTrans = Target.transform as RectTransform;
			AnchoredPosition = rectTrans.anchoredPosition;
		}

		protected void RecordSizeDelta()
		{
			RectTransform rectTrans = Target.transform as RectTransform;
			SizeDelta = rectTrans.sizeDelta;
		}

		protected void RecordPivot()
		{
			RectTransform rectTrans = Target.transform as RectTransform;
			Pivot = rectTrans.pivot;
		}

		public override void Record()
		{
			RecordAnchorMin();
			RecordAnchorMax();
			RecordAnchoredPosition();
			RecordSizeDelta();
			RecordPivot();
		}

		public override void Apply()
		{
			RectTransform rectTrans = Target.transform as RectTransform;
			rectTrans.anchorMin = AnchorMin;
			rectTrans.anchorMax = AnchorMax;
			rectTrans.anchoredPosition = AnchoredPosition;
			rectTrans.sizeDelta = SizeDelta;
			rectTrans.pivot = Pivot;
		}

		public override void CopyFrom(StatusSwitcherStatusBase other)
		{
			RectTransformStatus otherStatus = other as RectTransformStatus;
			AnchorMin = otherStatus.AnchorMin;
			AnchorMax = otherStatus.AnchorMax;
			AnchoredPosition = otherStatus.AnchoredPosition;
			SizeDelta = otherStatus.SizeDelta;
			Pivot = otherStatus.Pivot;
		}

#if UNITY_EDITOR
		protected void ShowAnchorPresetPopup(RectTransform rectTransform)
		{
			// 获取 RectTransformEditor 类型
			Type rectTransformEditorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.LayoutDropdownWindow");
			if (rectTransformEditorType == null)
			{
				Debug.LogError("无法找到 LayoutDropdownWindow 类型");
				return;
			}

			try
			{
				Rect dropdownPosition = GUILayoutUtility.GetRect(0, 0);
				dropdownPosition.x += 2;
				dropdownPosition.y += 17;
				dropdownPosition.height = 49;
				dropdownPosition.width = 49;

				UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(Target.transform);

				object obj = Activator.CreateInstance(rectTransformEditorType, new object[] { serializedObject });
				UnityEditor.PopupWindowContent anchorPresetPopup = obj as UnityEditor.PopupWindowContent;
				UnityEditor.PopupWindow.Show(dropdownPosition, anchorPresetPopup);
			}
			catch(Exception e)
			{
				Log.Error($"ShowAnchorPresetPopup failed, {e}", nameof(RectTransformStatus));
			}
			finally
			{

			}
		}
	}
#endif
}
