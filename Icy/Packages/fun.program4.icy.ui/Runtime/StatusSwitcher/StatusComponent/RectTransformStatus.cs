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
using System.Reflection;
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
#if UNITY_EDITOR
		[OnInspectorGUI(nameof(DrawAnchorIcon), true)]
		[PropertySpace(-20, 50)]
		[DisplayAsString]
		[HideLabel]
		protected string dummy;
#endif

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
		protected MethodInfo _DrawLayoutMode;
		protected UnityEditor.SerializedObject _SerializedObject;
		protected UnityEditor.PopupWindowContent _AnchorPresetdPopup;

		/// <summary>
		/// 显示RectTransform的锚框选择窗口
		/// </summary>
		protected void ShowAnchorPresetPopup(RectTransform rectTransform)
		{
			// 获取 RectTransformEditor 类型
			Type rectTransformEditorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.LayoutDropdownWindow");
			if (rectTransformEditorType == null)
			{
				Debug.LogError("Can not GetType LayoutDropdownWindow");
				return;
			}

			try
			{
				Rect dropdownPosition = GUILayoutUtility.GetRect(0, 0);
				dropdownPosition.x += 26;
				dropdownPosition.y -= 110;
				dropdownPosition.height = 49;
				dropdownPosition.width = 49;

				UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(Target.transform);

				object obj = Activator.CreateInstance(rectTransformEditorType, new object[] { serializedObject });
				_AnchorPresetdPopup = obj as UnityEditor.PopupWindowContent;
				UnityEditor.PopupWindow.Show(dropdownPosition, _AnchorPresetdPopup);
			}
			catch (ExitGUIException)
			{
				//这里会报一个这个异常，不影响使用，不输出log了
			}
			catch (Exception e)
			{
				Log.Error($"ShowAnchorPresetPopup exception, {e}", nameof(RectTransformStatus));
			}

			UnityEditor.EditorApplication.update -= CheckAnchorPresetdPopupStatus;
			UnityEditor.EditorApplication.update += CheckAnchorPresetdPopupStatus;
		}

		/// <summary>
		/// 显示RectTransform那个锚框图标
		/// </summary>
		protected void DrawAnchorIcon()
		{
			Rect dropdownPosition = GUILayoutUtility.GetRect(0, 0);
			dropdownPosition.height = 49;
			dropdownPosition.width = 49;

			using (new UnityEditor.EditorGUI.DisabledScope(false))
			{
				if (UnityEditor.EditorGUI.DropdownButton(dropdownPosition, GUIContent.none, FocusType.Passive, "label"))
				{
					GUIUtility.keyboardControl = 0;
					ShowAnchorPresetPopup(Target.transform as RectTransform);
				}
			}

			if (_DrawLayoutMode == null)
			{
				Type rectTransformEditorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.LayoutDropdownWindow");
				Type spType = typeof(UnityEditor.SerializedProperty);
				_DrawLayoutMode = rectTransformEditorType.GetMethod("DrawLayoutMode", BindingFlags.Static | BindingFlags.NonPublic, null
					, new Type[] { typeof(Rect), spType, spType, spType, spType }, null);
			}

			if (_SerializedObject == null)
				_SerializedObject = new UnityEditor.SerializedObject(Target.transform);

			UnityEditor.SerializedProperty m_AnchorMin = _SerializedObject.FindProperty("m_AnchorMin");
			UnityEditor.SerializedProperty m_AnchorMax = _SerializedObject.FindProperty("m_AnchorMax");
			UnityEditor.SerializedProperty m_AnchoredPosition = _SerializedObject.FindProperty("m_AnchoredPosition");
			UnityEditor.SerializedProperty m_SizeDelta = _SerializedObject.FindProperty("m_SizeDelta");
			UnityEditor.SerializedProperty m_Pivot = _SerializedObject.FindProperty("m_Pivot");

			object[] typeArgs = new object[] { new RectOffset(7, 7, 7, 7).Remove(dropdownPosition)
											, m_AnchorMin, m_AnchorMax, m_AnchoredPosition, m_SizeDelta };
			_DrawLayoutMode.Invoke(null, typeArgs);
		}

		/// <summary>
		/// 检查AnchorPresetdPopup窗口状态，监听关闭时机
		/// </summary>
		protected void CheckAnchorPresetdPopupStatus()
		{
			if (_AnchorPresetdPopup == null || _AnchorPresetdPopup.editorWindow == null)
			{
				UnityEditor.EditorApplication.update -= CheckAnchorPresetdPopupStatus;
				OnAnchorPresetdPopupClosed();
			}
		}

		protected void OnAnchorPresetdPopupClosed()
		{
			Record();
		}
#endif
	}
}
