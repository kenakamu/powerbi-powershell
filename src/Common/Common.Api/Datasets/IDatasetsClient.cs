﻿/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

using System.Collections.Generic;

namespace Microsoft.PowerBI.Common.Api.Datasets
{
    public interface IDatasetsClient
    {
        IEnumerable<Dataset> GetDatasets();
        IEnumerable<Dataset> GetDatasetsAsAdmin(string filter = default, int? top = default, int? skip = default);
    }
}