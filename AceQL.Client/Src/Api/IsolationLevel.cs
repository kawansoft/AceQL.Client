/*
 * This file is part of AceQL C# Client SDK.
 * AceQL C# Client SDK: Remote SQL access over HTTP with AceQL HTTP.                                 
 * Copyright (C) 2020,  KawanSoft SAS
 * (http://www.kawansoft.com). All rights reserved.                                
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
﻿

namespace AceQL.Client.Api
{
    /// <summary>
    /// Specifies the transaction locking behavior for the remote connection.
    /// </summary>
    public enum IsolationLevel
    {
        /// <summary>
        /// A different isolation level than the one specified is being used, but the level cannot be determined.
        /// </summary>
        Unspecified = -1,

        /// <summary>
        /// A dirty read is possible, meaning that no shared locks are issued and no exclusive locks are honored.
        /// </summary>
        ReadUncommitted = 256,

        /// <summary>
        /// Shared locks are held while the data is being read to avoid dirty reads, but the data can be changed before the end of the transaction, resulting in non-repeatable reads or phantom data.
        /// </summary>
        ReadCommitted = 4096,

        /// <summary>
        /// Locks are placed on all data that is used in a query, preventing other users from updating the data. Prevents non-repeatable reads but phantom rows are still possible.
        /// </summary>
        RepeatableRead = 65536,
 
        /// <summary>
        ///A range lock is placed, preventing other users from updating or inserting rows into the dataset until the transaction is complete.
        /// </summary>
        Serializable = 1048576,
    }
}
