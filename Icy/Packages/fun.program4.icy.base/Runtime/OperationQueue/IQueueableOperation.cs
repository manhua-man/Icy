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

namespace Icy.Base
{
	/// <summary>
	/// 实现此接口的类，可以加入到OperationQueue队列中，以实现异步按顺序执行
	/// </summary>
	public interface IQueueableOperation
	{
		/// <summary>
		/// 执行此Operation，执行完成后，OperationQueue会继续执行下一个Operation
		/// </summary>
		public abstract UniTask Execute();
	}
}
