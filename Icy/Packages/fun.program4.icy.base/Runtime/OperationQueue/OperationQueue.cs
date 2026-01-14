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


using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Icy.Base
{
	/// <summary>
	/// 异步操作队列，支持按顺序执行一组异步Operation；
	/// 入队的Operation可以是IQueueableOperation，或者Func<UniTask>、async ()=> {}
	/// </summary>
	public class OperationQueue : IDisposable
	{
		/// <summary>
		/// 一个预设的、常驻的异步操作队列，方便业务侧直接使用
		/// </summary>
		public static OperationQueue Default { get; internal set; }
		/// <summary>
		/// 队列当前有多少个Operation，包括正在执行的
		/// </summary>
		public int Count => _QueueTypeQueue.Count;
		/// <summary>
		/// Dispose完成的事件，参数是队列中剩余未执行的Operation数量
		/// </summary>
		public event Action<int> OnDisposed;
		/// <summary>
		/// 执行Operation时异常的事件
		/// </summary>
		public event Action<Exception> OnException;

		/// <summary>
		/// 队列的名字
		/// </summary>
		protected string _Name;
		/// <summary>
		/// IQueueableOperation类型的队列
		/// </summary>
		protected Queue<IQueueableOperation> _QueueableOperationQueue;
		/// <summary>
		/// Func<UniTask> 或 async ()=> {}类型的队列
		/// </summary>
		protected Queue<Func<UniTask>> _FuncUniTaskQueue;
		/// <summary>
		/// 使前两个队列正确合作执行的队列
		/// </summary>
		protected Queue<QueueType> _QueueTypeQueue;
		/// <summary>
		/// 是否正在执行一个Operation
		/// </summary>
		protected bool _IsExecuting;
		/// <summary>
		/// 是否已经调用了Dispose
		/// </summary>
		protected bool _DisposeRequested;


		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="name">队列的名字，最好唯一</param>
		/// <param name="initialCapacity">初始队列大小</param>
		public OperationQueue(string name, int initialCapacity = 8)
		{
			_Name = name;
			_QueueableOperationQueue = new Queue<IQueueableOperation>(initialCapacity);
			_FuncUniTaskQueue = new Queue<Func<UniTask>>(initialCapacity);
			_QueueTypeQueue = new Queue<QueueType>(initialCapacity);
			_IsExecuting = false;
			_DisposeRequested = false;
		}

		/// <summary>
		/// 入队一个QueueableOperation的实现类
		/// </summary>
		public bool Enqueue(IQueueableOperation operation)
		{
			if (operation == null)
			{
				Log.Error($"Enqueue {nameof(IQueueableOperation)} is null", nameof(OperationQueue));
				return false;
			}

			if (_DisposeRequested)
				return false;

			_QueueableOperationQueue.Enqueue(operation);
			_QueueTypeQueue.Enqueue(QueueType.QueueableOperation);

			if (!_IsExecuting)
				ExecuteOne().Forget();

			return true;
		}

		/// <summary>
		/// 入队一个Func<UniTask>，或者 async () => {}
		/// </summary>
		public bool Enqueue(Func<UniTask> func)
		{
			if (func == null)
			{
				Log.Error($"Enqueue {nameof(Func<UniTask>)} is null", nameof(OperationQueue));
				return false;
			}

			if (_DisposeRequested)
				return false;

			_FuncUniTaskQueue.Enqueue(func);
			_QueueTypeQueue.Enqueue(QueueType.FuncUniTask);

			if (!_IsExecuting)
				ExecuteOne().Forget();

			return true;
		}

		/// <summary>
		/// 执行一个Operation
		/// </summary>
		protected async UniTask ExecuteOne()
		{
			_IsExecuting = true;
			QueueType queueType = _QueueTypeQueue.Dequeue();
			try
			{
				switch (queueType)
				{
					case QueueType.QueueableOperation:
						IQueueableOperation operation = _QueueableOperationQueue.Dequeue();
						await operation.Execute();
						break;
					case QueueType.FuncUniTask:
						await _FuncUniTaskQueue.Dequeue().Invoke();
						break;
				}
			}
			catch (Exception e)
			{
				Log.Error($"Execute {queueType} exception: {e}", nameof(OperationQueue));
				OnException?.Invoke(e);
			}
			finally
			{
				if (_DisposeRequested)
				{
					_IsExecuting = false;
					if (Count > 0)
						Log.Warn($"{nameof(OperationQueue)} {_Name} disposed with {Count} unexecuted operation(s) ", nameof(OperationQueue), true);
					OnDisposed?.Invoke(Count);
				}
				else if (Count > 0)
					ExecuteOne().Forget();
			}
		}

		/// <summary>
		/// 如果当前有正在执行的Operation，并不会立刻Dispose掉，而是等到当前正在执行的Operation结束后；
		/// 队列中未执行的Operation不会再被执行
		/// </summary>
		public void Dispose()
		{
			_DisposeRequested = true;
			Log.Info($"{nameof(OperationQueue)} {_Name} disposing is requested", nameof(OperationQueue), true);
		}


		protected enum QueueType
		{
			QueueableOperation,
			FuncUniTask,
		}
	}
}
