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
using LitMotion.Animation;
using Sirenix.OdinInspector;

namespace Icy.UI
{
	/// <summary>
	/// LitMotion状态，记录针对LitMotionAnimation的控制操作
	/// </summary>
	[HideLabel]
	[System.Serializable]
	public class LitMotionStatus : StatusSwitcherStatusBase
	{
		[EnumToggleButtons]
		public OperationType Operation;

		public enum OperationType
		{
			Play,
			Pause,
			Stop,
			Restart,
		}

		protected LitMotionAnimation _LitMotionAnim;

		public override void Init(StatusSwitcherTarget target)
		{
			base.Init(target);
			_LitMotionAnim = target.GetComponent<LitMotionAnimation>();
		}

		public override void Record()
		{
		}

		public override void Apply()
		{
			switch (Operation)
			{
				case OperationType.Play:
					_LitMotionAnim.Play();
					break;
				case OperationType.Pause:
					_LitMotionAnim.Pause();
					break;
				case OperationType.Stop:
					_LitMotionAnim.Stop();
					break;
				case OperationType.Restart:
					_LitMotionAnim.Restart();
					break;
				default:
					Log.Error(Operation, nameof(LitMotionStatus));
					break;
			}
		}

		public override void CopyFrom(StatusSwitcherStatusBase other)
		{
			LitMotionStatus otherStatus = other as LitMotionStatus;
			Operation = otherStatus.Operation;
		}

		public override void Dispose()
		{
			//Editor下退出编辑时，需要停止LitMotion，否则物体销毁了、LitMotion还在执行，会持续报错
			if (_LitMotionAnim != null)
				_LitMotionAnim.Stop();
			base.Dispose();
		}
	}
}
