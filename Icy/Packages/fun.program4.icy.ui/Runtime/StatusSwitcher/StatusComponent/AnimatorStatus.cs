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
using UnityEngine;

namespace Icy.UI
{
	/// <summary>
	/// Animator状态，记录是否播放动画、播放的动画State、播放速度、是否从头播放
	/// </summary>
	[HideLabel]
	[System.Serializable]
	public class AnimatorStatus : StatusSwitcherStatusBase
	{
		/// <summary>
		/// 是否播放动画（控制Animator组件的enable）
		/// </summary>
		[InlineProperty]
		[SerializeField]
		public bool Play;

		/// <summary>
		/// 要播放的动画State名字
		/// </summary>
		[InlineProperty]
		[SerializeField]
		[DelayedProperty]
		[ShowIf(nameof(Play))]
		public string StateNameToPlay;

		/// <summary>
		/// 播放速度
		/// </summary>
		[InlineProperty]
		[SerializeField]
		[DelayedProperty]
		[ShowIf(nameof(Play))]
		public float Speed = 1.0f;

		/// <summary>
		/// 是否从头播放
		/// </summary>
		[InlineProperty]
		[SerializeField]
		public bool Rewind;

		protected Animator _Animator;

		public override void Init(StatusSwitcherTarget target)
		{
			base.Init(target);
			_Animator = target.GetComponent<Animator>();
		}

		public override void Record()
		{
			Speed = _Animator.speed;
		}

		public override void Apply()
		{
			if (Play && string.IsNullOrEmpty(StateNameToPlay))
			{
				Log.Error($"{Target.gameObject.name} has a empty StateNameToPlay", nameof(AnimatorStatus));
			}
			else
			{
				if (Play)
				{
					_Animator.enabled = true;
					_Animator.speed = Speed;
					if (Rewind)
						_Animator.Play(StateNameToPlay, -1, 0.0f);
					else
						_Animator.Play(StateNameToPlay);
				}
				else
					_Animator.enabled = false;
			}
		}

		public override void CopyFrom(StatusSwitcherStatusBase other)
		{
			AnimatorStatus otherStatus = other as AnimatorStatus;
			Play = otherStatus.Play;
			StateNameToPlay = otherStatus.StateNameToPlay;
			Speed = otherStatus.Speed;
			Rewind = otherStatus.Rewind;
		}
	}
}
