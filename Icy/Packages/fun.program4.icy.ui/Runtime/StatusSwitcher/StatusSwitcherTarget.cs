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
using System.Collections.Generic;
using UnityEngine;

namespace Icy.UI
{
	/// <summary>
	/// 受StatusSwitcher控制的目标，会序列化保存各种状态，由StatusSwitcher驱动切换
	/// </summary>
	[HideMonoScript]
	public class StatusSwitcherTarget : MonoBehaviour
	{
#if UNITY_EDITOR
		/// <summary>
		/// StatusItem下拉列表的数据源，为拼接的StatusSwitcher + Status名字
		/// </summary>
		protected List<string> _StatusItems;

		/// <summary>
		/// 用下拉列表选择StatusItem
		/// </summary>
		[Title("所属StatusItem")]
		[ValueDropdown(nameof(_StatusItems), IsUniqueList = true, DropdownWidth = 200)]
		[OnValueChanged(nameof(OnStatusItemDropdownChanged))]
		[ShowIf(nameof(HasAddedToAnyStatusItem))]
		[ShowInInspector]
		internal string StatusItemName = NONE;
#endif

		[PropertySpace(10, 10)]
		[Title("此节点记录的状态类型")]
#if UNITY_EDITOR
		[ShowIf(nameof(NeedShowRecordTypes))]
		[OnInspectorInit(nameof(Init))]
		[OnValueChanged(nameof(OnRecordTypesChanged))]
#endif
		public StatusSwitcherRecordType RecordTypes;

		protected StatusSwitcherRecordType _PrevRecordTypes;

		/// <summary>
		/// GameObject状态
		/// </summary>
#if UNITY_EDITOR
		[ShowIf(nameof(NeedShowGameObject))]
#endif
		[TitleGroup("Status List", "修改这里的值会自动同步到物体上，修改物体上的值《不会》自动同步到这里，需要自己点一下右侧的Record按钮")]
		[BoxGroup("Status List/GameObject")]
		public GameObjectStatus GameObjectStatus;

		/// <summary>
		/// Transform状态
		/// </summary>
#if UNITY_EDITOR
		[ShowIf(nameof(NeedShowTransform))]
#endif
		[BoxGroup("Status List/Transform")]
		public TransformStatus TransformStatus;

		/// <summary>
		/// RectTransform状态
		/// </summary>
#if UNITY_EDITOR
		[ShowIf(nameof(NeedShowRectTransform))]
#endif
		[BoxGroup("Status List/RectTransform")]
		public RectTransformStatus RectTransformStatus;

		/// <summary>
		/// Animator状态
		/// </summary>
#if UNITY_EDITOR
		[ShowIf(nameof(NeedShowAnimator))]
#endif
		[BoxGroup("Status List/Animator")]
		public AnimatorStatus AnimatorStatus;

		/// <summary>
		/// LitMotion状态
		/// </summary>
#if UNITY_EDITOR
		[ShowIf(nameof(NeedShowLitMotion))]
#endif
		[BoxGroup("Status List/LitMotion")]
		public LitMotionStatus LitMotionStatus;

		//New Status stub

#if UNITY_EDITOR
		/// <summary>
		/// 受这些StatusSwitcher的控制
		/// </summary>
		[Title("所属StatusSwitcher列表（双击可跳转）")]
		[ListDrawerSettings(ShowItemCount = true, ShowFoldout = false, IsReadOnly = true)]
		[ShowIf(nameof(HasAddedToAnyStatusItem))]
		[PropertySpace(10, 20)]
		[ShowInInspector]
		protected List<StatusSwitcher> StatusSwitchers;

		[ShowInInspector]
		[ShowIf(nameof(HasAddedToAnyStatusItem))]
		[LabelWidth(50)]
		protected bool _Debug = false;
#endif

		/// <summary>
		/// 节点的记录数据
		/// </summary>
		[SerializeField]
#if UNITY_EDITOR
		[ShowIf(nameof(_Debug))]
#endif
		[ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = false, HideAddButton = true)]
		internal List<StatusSwitcherRecord> Records;


		/// <summary>
		/// 切换到指定StatusItem
		/// </summary>
		internal bool SwitchTo(StatusSwitcherItem statusItem)
		{
			bool rtn = true;
			for (int i = 0; i < Records.Count; i++)
			{
				StatusSwitcherItem item = Records[i].StatusItem;
				if (item == statusItem)
				{
					rtn &= TryToApply(i, StatusSwitcherRecordType.GameObject);
					rtn &= TryToApply(i, StatusSwitcherRecordType.Transform);
					rtn &= TryToApply(i, StatusSwitcherRecordType.RectTransform);
					rtn &= TryToApply(i, StatusSwitcherRecordType.Animator);
					rtn &= TryToApply(i, StatusSwitcherRecordType.LitMotion);

					//New Status stub
					break;
				}
			}
			return rtn;
		}

		protected bool TryToApply(int idx, StatusSwitcherRecordType recordType)
		{
			StatusSwitcherRecord record = Records[idx];
			if (record.RecordTypes.HasFlag(recordType))
			{
				if (this == null)
				{
					Log.Error($"Apply {recordType} failed, is target gameObject deleted? {nameof(StatusSwitcher)} = {record.StatusItem.GetSwitcherName()}, status name = {record.StatusItem.Name}", nameof(StatusSwitcherTarget));
					return false;
				}

				StatusSwitcherStatusBase status = GetStatusByType(record, recordType);
				status.Init(this);
				status.Apply();
				return true;
			}
			return true;
		}

		protected StatusSwitcherStatusBase GetStatusByType(StatusSwitcherRecord record, StatusSwitcherRecordType recordType)
		{
			switch (recordType)
			{
				case StatusSwitcherRecordType.GameObject:
					return record.AllStatusSwitcherComponent.gameObject;
				case StatusSwitcherRecordType.Transform:
					return record.AllStatusSwitcherComponent.transform;
				case StatusSwitcherRecordType.RectTransform:
					return record.AllStatusSwitcherComponent.rectTransform;
				case StatusSwitcherRecordType.Animator:
					return record.AllStatusSwitcherComponent.animator;
				case StatusSwitcherRecordType.LitMotion:
					return record.AllStatusSwitcherComponent.litMotion;
				//New Status stub
				default:
					return null;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// StatusItem下拉列表的空选项
		/// </summary>
		internal const string NONE = "None";

		protected void Init()
		{
			_PotentialStatusSwitchers = new List<StatusSwitcher>();
			StatusSwitchers = new List<StatusSwitcher>();
			Transform uiBaseTrans = CommonUtility.GetAncestor(transform, FindUIBase);
			if (uiBaseTrans != null)
			{
				StatusSwitcher[] switchers = uiBaseTrans.GetComponentsInChildren<StatusSwitcher>();
				for (int sw = 0; sw < switchers.Length; sw++)
				{
					List<StatusSwitcherItem> list = switchers[sw].StatusList;
					for (int s = 0; s < list.Count; s++)
					{
						List<StatusSwitcherTarget> targets = list[s].Targets;
						for (int t = 0; t < targets.Count; t++)
						{
							if (targets[t] == this)
							{
								//所属的StatusSwitcher
								if (!StatusSwitchers.Contains(switchers[sw]))
									StatusSwitchers.Add(switchers[sw]);

								//尝试添加一个Record
								TryToAddRecord(list[s]);
							}
						}
					}

					_PotentialStatusSwitchers.Add(switchers[sw]);
				}
			}

			//初始化StatusItem下拉列表
			_StatusItems = new List<string>();
			_StatusItems.Add(NONE); //以在下拉列表里显示一个可以置空的选项
			if (Records != null)
			{
				for (int i = 0; i < Records.Count; i++)
				{
					StatusSwitcherItem item = Records[i].StatusItem;
					string key = $"{item.StatusSwitcher.gameObject.name} - {item.Name}";
					_StatusItems.Add(key);
				}
			}

			_StatusSwitcherToAdd = null;
			_StatusSwitcherItemToAdd = null;
			_Debug = false;
		}

		/// <summary>
		/// 尝试添加一个Record
		/// </summary>
		protected void TryToAddRecord(StatusSwitcherItem statusItem)
		{
			if (Records == null)
				Records = new List<StatusSwitcherRecord>();

			bool has = false;
			for (int i = 0; i < Records.Count; i++)
			{
				if (Records[i].StatusItem == statusItem)
				{
					has = true;
					break;
				}
			}

			if (!has)
			{
				StatusSwitcherRecord newRecord = new StatusSwitcherRecord();
				newRecord.StatusItem = statusItem;
				newRecord.RecordTypes = StatusSwitcherRecordType.None;
				newRecord.AllStatusSwitcherComponent = new AllStatusSwitcherComponent();
				Records.Add(newRecord);
			}
		}

		protected void OnStatusItemDropdownChanged()
		{
			if (StatusItemName == NONE)
			{
				_PrevRecordTypes = StatusSwitcherRecordType.None;
				Clear();
			}
			else
			{
				int idx = _StatusItems.IndexOf(StatusItemName);
				idx -= 1;//因为_StatusItems第一个元素是None，所以这里要减1
				StatusSwitcherRecord record = Records[idx];
				RecordTypes = record.RecordTypes;
				_PrevRecordTypes = record.RecordTypes;

				ForEachRecordTypes((StatusSwitcherRecordType recordType) =>
				{
					InitStatus(record, recordType);
				});
			}
		}

		protected void OnRecordTypesChanged()
		{
			int idx = _StatusItems.IndexOf(StatusItemName);
			idx -= 1;//因为_StatusItems第一个元素是None，所以这里要减1
			StatusSwitcherRecord record = Records[idx];

			ForEachRecordTypes((StatusSwitcherRecordType recordType) =>
			{
				if (!_PrevRecordTypes.HasFlag(recordType))
					InitStatus(record, recordType);
			});

			_PrevRecordTypes = RecordTypes;
		}

		protected void InitStatus(StatusSwitcherRecord record, StatusSwitcherRecordType recordType)
		{
			bool has = RecordTypes.HasFlag(recordType);
			switch (recordType)
			{
				case StatusSwitcherRecordType.GameObject:
					if (has)
					{
						GameObjectStatus = new GameObjectStatus();
						InitStatusSingle(GameObjectStatus, StatusSwitcherRecordType.GameObject
							, record.AllStatusSwitcherComponent.gameObject);
					}
					else
						GameObjectStatus = null;
					break;
				case StatusSwitcherRecordType.Transform:
					if (has)
					{
						TransformStatus = new TransformStatus();
						InitStatusSingle(TransformStatus, StatusSwitcherRecordType.Transform
							, record.AllStatusSwitcherComponent.transform);
					}
					else
						TransformStatus = null;
					break;
				case StatusSwitcherRecordType.RectTransform:
					if (has)
					{
						RectTransformStatus = new RectTransformStatus();
						InitStatusSingle(RectTransformStatus, StatusSwitcherRecordType.RectTransform
							, record.AllStatusSwitcherComponent.rectTransform);
					}
					else
						RectTransformStatus = null;
					break;
				case StatusSwitcherRecordType.Animator:
					if (has)
					{
						AnimatorStatus = new AnimatorStatus();
						InitStatusSingle(AnimatorStatus, StatusSwitcherRecordType.Animator
							, record.AllStatusSwitcherComponent.animator);
					}
					else
						AnimatorStatus = null;
					break;
				case StatusSwitcherRecordType.LitMotion:
					if (has)
					{
						LitMotionStatus = new LitMotionStatus();
						InitStatusSingle(LitMotionStatus, StatusSwitcherRecordType.LitMotion
							, record.AllStatusSwitcherComponent.litMotion);
					}
					else
						LitMotionStatus = null;
					break;
				//New Status stub
				default:
					break;
			}
		}

		protected void InitStatusSingle(StatusSwitcherStatusBase status, StatusSwitcherRecordType recordType, StatusSwitcherStatusBase statusSaved)
		{
			status.Init(this);
			if (!_PrevRecordTypes.HasFlag(recordType))
				status.Record();
			else
			{
				status.CopyFrom(statusSaved);
				status.Apply();
			}
		}

		/// <summary>
		/// 将当前编辑的状态数据序列化存储起来
		/// </summary>
		[PropertySpace(10)]
		[ShowIf(nameof(NeedShowRecordTypes))]
		[Button("Save", Icon = SdfIconType.SdCardFill, ButtonHeight = (int)ButtonSizes.Medium), GUIColor(0, 1, 0)]
		protected void Save()
		{
			int idx = _StatusItems.IndexOf(StatusItemName);
			idx -= 1;//因为_StatusItems第一个元素是None，所以这里要减1
			StatusSwitcherRecord record = Records[idx];
			record.RecordTypes = RecordTypes;

			ForEachRecordTypes((StatusSwitcherRecordType recordType) => 
			{
				SaveSingle(record, recordType);
			});
		}

		protected void SaveSingle(StatusSwitcherRecord record, StatusSwitcherRecordType recordType)
		{
			bool has = RecordTypes.HasFlag(recordType);
			switch (recordType)
			{
				case StatusSwitcherRecordType.GameObject:
					record.AllStatusSwitcherComponent.gameObject = has ? GameObjectStatus : null;
					break;
				case StatusSwitcherRecordType.Transform:
					record.AllStatusSwitcherComponent.transform = has ? TransformStatus : null;
					break;
				case StatusSwitcherRecordType.RectTransform:
					record.AllStatusSwitcherComponent.rectTransform = has ? RectTransformStatus : null;
					break;
				case StatusSwitcherRecordType.Animator:
					record.AllStatusSwitcherComponent.animator = has ? AnimatorStatus : null;
					break;
				case StatusSwitcherRecordType.LitMotion:
					record.AllStatusSwitcherComponent.litMotion = has ? LitMotionStatus : null;
					break;
				//New Status stub
				default:
					break;
			}
		}

		#region 添加到StatusSwitcher
		/// <summary>
		/// UI所有的StatusSwitcher
		/// </summary>
		protected List<StatusSwitcher> _PotentialStatusSwitchers;

		[Title("选择StatusSwitcher")]
		[ValueDropdown(nameof(_PotentialStatusSwitchers), IsUniqueList = true, DropdownWidth = 200)]
		[OnValueChanged(nameof(OnStatusSwitcherSelected))]
		[HideIf(nameof(HasAddedToAnyStatusItem))]
		[ShowInInspector]
		protected StatusSwitcher _StatusSwitcherToAdd;

		/// <summary>
		/// 当前选择的StatusSwitcher的所有StatusSwitcherItem
		/// </summary>
		protected List<string> _PotentialStatusSwitcherItems;

		[Title("选择StatusSwitcherItem")]
		[ValueDropdown(nameof(_PotentialStatusSwitcherItems), IsUniqueList = true, DropdownWidth = 200)]
		[HideIf(nameof(HasAddedToAnyStatusItem))]
		[ShowInInspector]
		protected string _StatusSwitcherItemToAdd;

		protected void OnStatusSwitcherSelected()
		{
			_PotentialStatusSwitcherItems = new List<string>();
			for (int i = 0; i < _StatusSwitcherToAdd.StatusList.Count; i++)
				_PotentialStatusSwitcherItems.Add(_StatusSwitcherToAdd.StatusList[i].Name);
			_StatusSwitcherItemToAdd = null;
		}

		/// <summary>
		/// 将本Target，添加到一个StatusSwitcher的StatusItem中
		/// </summary>
		[PropertySpace(10)]
		[HideIf(nameof(HasAddedToAnyStatusItem))]
		[Button("Add To StatusItem", Icon = SdfIconType.PlusCircleFill, ButtonHeight = (int)ButtonSizes.Medium)]
		protected void AddToStatusItem()
		{
			for (int i = 0; i < _StatusSwitcherToAdd.StatusList.Count; i++)
			{
				if (_StatusSwitcherToAdd.StatusList[i].Name == _StatusSwitcherItemToAdd)
				{
					_StatusSwitcherToAdd.StatusList[i].Targets.Add(this);

					StatusItemName = null;
					RecordTypes = StatusSwitcherRecordType.None;
					Init();
					break;
				}
			}
		}
		#endregion

		protected void ForEachRecordTypes(Action<StatusSwitcherRecordType> callback)
		{
			Array enumValues = Enum.GetValues(typeof(StatusSwitcherRecordType));
			foreach (StatusSwitcherRecordType e in enumValues)
			{
				if (e != StatusSwitcherRecordType.None)
					callback(e);
			}
		}

		protected bool FindUIBase(Transform trans)
		{
			if (trans.GetComponent<UIBase>() != null)
				return true;
			return false;
		}

		protected bool HasAddedToAnyStatusItem()
		{
			return _StatusItems!= null && _StatusItems.Count > 1; //里面默认有一个NONE，所以是1
		}

		protected bool NeedShowRecordTypes()
		{
			return !string.IsNullOrEmpty(StatusItemName) && StatusItemName != NONE && HasAddedToAnyStatusItem();
		}

		protected bool NeedShowGameObject()
		{
			return RecordTypes.HasFlag(StatusSwitcherRecordType.GameObject) && NeedShowRecordTypes();
		}

		protected bool NeedShowTransform()
		{
			return RecordTypes.HasFlag(StatusSwitcherRecordType.Transform) && NeedShowRecordTypes();
		}

		protected bool NeedShowRectTransform()
		{
			return RecordTypes.HasFlag(StatusSwitcherRecordType.RectTransform) && NeedShowRecordTypes();
		}

		protected bool NeedShowAnimator()
		{
			return RecordTypes.HasFlag(StatusSwitcherRecordType.Animator) && NeedShowRecordTypes();
		}

		protected bool NeedShowLitMotion()
		{
			return RecordTypes.HasFlag(StatusSwitcherRecordType.LitMotion) && NeedShowRecordTypes();
		}

		//New Status stub
#endif

		protected void Clear()
		{
			GameObjectStatus?.Dispose();
			TransformStatus?.Dispose();
			RectTransformStatus?.Dispose();
			AnimatorStatus?.Dispose();
			LitMotionStatus?.Dispose();
			GameObjectStatus = null;
			TransformStatus = null;
			RectTransformStatus = null;
			AnimatorStatus = null;
			LitMotionStatus = null;
			//New Status stub
		}
	}

	/// <summary>
	/// 所有状态的Flag
	/// </summary>
	[Flags]
	[Serializable]
	public enum StatusSwitcherRecordType
	{
		None = 0,
		GameObject = 1 << 0,
		Transform = 1 << 1,
		RectTransform = 1 << 2,
		//ImageEx = 1 << 3,
		//TextEx = 1 << 4,
		//ButtonEx = 1 << 5,
		Animator = 1 << 6,
		LitMotion = 1 << 7,

		//New Status stub
	}

	/// <summary>
	/// 一条Record，对应一个StatusItem
	/// </summary>
	[Serializable]
	public class StatusSwitcherRecord
	{
		/// <summary>
		/// 所属StatusItem
		/// </summary>
		public StatusSwitcherItem StatusItem;
		/// <summary>
		/// 都存储了哪些类型的组件数据
		/// </summary>
		public StatusSwitcherRecordType RecordTypes;
		/// <summary>
		/// 所有组件数据的集合
		/// </summary>
		public AllStatusSwitcherComponent AllStatusSwitcherComponent;
	}

	/// <summary>
	/// 所有组件数据的集合
	/// </summary>
	[Serializable]
	public class AllStatusSwitcherComponent
	{
		public GameObjectStatus gameObject;
		public TransformStatus transform;
		public RectTransformStatus rectTransform;
		public AnimatorStatus animator;
		public LitMotionStatus litMotion;
		//New Status stub
	}
}
