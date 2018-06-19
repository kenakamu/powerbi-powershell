﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.PowerBI.Common.Abstractions;
using Microsoft.PowerBI.Common.Abstractions.Interfaces;
using Microsoft.PowerBI.Common.Api.Datasets;
using Microsoft.PowerBI.Common.Client;

namespace Microsoft.PowerBI.Commands.Data
{
    [Cmdlet(CmdletVerb, CmdletName, DefaultParameterSetName = ListParameterSetName)]
    public class GetPowerBIDatasource : PowerBIClientCmdlet, IUserScope
    {
        public const string CmdletVerb = VerbsCommon.Get;
        public const string CmdletName = "PowerBIDatasource";

        private const string IdParameterSetName = "Id";
        private const string NameParameterSetName = "Name";
        private const string ListParameterSetName = "List";
        private const string ObjectIdParameterSetName = "ObjectAndId";
        private const string ObjectNameParameterSetName = "ObjectAndName";
        private const string ObjectListParameterSetName = "ObjectAndList";

        public GetPowerBIDatasource() : base() { }

        public GetPowerBIDatasource(IPowerBIClientCmdletInitFactory init) : base(init) { }

        #region Parameters
        [Alias("DatasetKey")]
        [Parameter(Mandatory = true, ParameterSetName = ListParameterSetName)]
        [Parameter(Mandatory = true, ParameterSetName = IdParameterSetName)]
        [Parameter(Mandatory = true, ParameterSetName = NameParameterSetName)]
        public Guid DatasetId { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ObjectIdParameterSetName, ValueFromPipeline = true)]
        [Parameter(Mandatory = true, ParameterSetName = ObjectNameParameterSetName, ValueFromPipeline = true)]
        [Parameter(Mandatory = true, ParameterSetName = ObjectListParameterSetName, ValueFromPipeline = true)]
        public Dataset Dataset { get; set; }

        [Alias("GroupId")]
        [Parameter(Mandatory = false)]
        public Guid WorkspaceId { get; set; }

        [Alias("DatasourceId")]
        [Parameter(Mandatory = true, ParameterSetName = IdParameterSetName)]
        public Guid Id { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = NameParameterSetName)]
        public string Name { get; set; }

        [Parameter(Mandatory = false)]
        public PowerBIUserScope Scope { get; set; } = PowerBIUserScope.Individual;
        #endregion

        public override void ExecuteCmdlet()
        {
            if(this.Dataset != null)
            {
                this.DatasetId = new Guid(this.Dataset.Id);
            }

            IEnumerable<Datasource> datasources = null;
            using (var client = this.CreateClient())
            {
                datasources = this.Scope == PowerBIUserScope.Individual ?
                    client.Datasets.GetDatasources(this.DatasetId, this.WorkspaceId) :
                    client.Datasets.GetDatasourcesAsAdmin(this.DatasetId);
            }

            if(this.Id != default)
            {
                datasources = datasources.Where(d => this.Id == new Guid(d.DatasourceId)).ToList();
            }

            if(!string.IsNullOrEmpty(this.Name))
            {
                datasources = datasources.Where(d => d.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            this.Logger.WriteObject(datasources, true);
        }
    }
}