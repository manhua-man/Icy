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


#if UNITY_EDITOR

using Cysharp.Threading.Tasks;

namespace Icy.Base
{
	public static class OperationQueueTest
	{
		public static void Test()
		{
			OperationQueue queue = new OperationQueue("TestOperationQueue"); //OperationQueue.Default;
			queue.OnDisposed += OnDisposed;

			// 1
			queue.Enqueue(async () =>
			{
				Log.Error("async ()=> {}，1");
				await UniTask.WaitForSeconds(1);
			});

			//queue.Dispose();

			// 2
			queue.Enqueue(new TestQueueableOperation());

			// 3
			queue.Enqueue(async () =>
			{
				//queue.Dispose();
				Log.Error("async ()=> {}，3");
				await UniTask.WaitForSeconds(1);
			});

		}

		public static void OnDisposed(int unexecutedCount)
		{
			Log.Error("OperationQueue disposed, unexecutedCount = " + unexecutedCount);
		}


		public class TestQueueableOperation : IQueueableOperation
		{
			public async UniTask Execute()
			{
				//throw new System.Exception("aa");
				Log.Error("QueueableOperation，2");
				await UniTask.WaitForSeconds(1);
			}
		}
	}
}
#endif
