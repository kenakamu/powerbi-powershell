﻿/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License.
 */

using System;
using System.IO;
using System.Management.Automation;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.PowerBI.Commands.Common;
using Microsoft.PowerBI.Common.Abstractions.Interfaces;

namespace Microsoft.PowerBI.Commands.Profile
{
    [Cmdlet(CmdletVerb, CmdletName)]
    [OutputType(typeof(string))]
    public class InvokePowerBIRestMethod : PowerBICmdlet
    {
        // Similiar to Invoke-RestMethod - https://github.com/PowerShell/PowerShell/blob/master/src/Microsoft.PowerShell.Commands.Utility/commands/utility/WebCmdlet/Common/InvokeRestMethodCommand.Common.cs

        public const string CmdletVerb = VerbsLifecycle.Invoke;
        public const string CmdletName = "PowerBIRestMethod";

        #region Constructors
        public InvokePowerBIRestMethod() : base() { }

        public InvokePowerBIRestMethod(IPowerBICmdletInitFactory init) : base(init) { }
        #endregion

        #region Parameters
        [Parameter(Mandatory = true)]
        public string Url { get; set; }

        [Parameter(Mandatory = true)]
        public PowerBIWebRequestMethod Method { get; set; }

        [Parameter(Mandatory = false)]
        public string Body { get; set; }

        [Parameter(Mandatory = false)]
        public string Organization = "myorg";

        [Parameter(Mandatory = false)]
        public string Version = "v1.0";

        [Parameter(Mandatory = false)]

        public virtual string OutFile { get; set; }
        #endregion

        public override void ExecuteCmdlet()
        {
            if(!string.IsNullOrEmpty(this.OutFile))
            {
                this.OutFile = this.ResolveFilePath(this.OutFile, false);
                if(File.Exists(this.OutFile))
                {
                    this.Logger.ThrowTerminatingError(new NotSupportedException($"OutFile '{this.OutFile}' already exists, specify a new file path"), ErrorCategory.InvalidArgument);
                }
            }

            if(Uri.TryCreate(this.Url, UriKind.Absolute, out Uri testUri))
            {
                this.Url = testUri.AbsoluteUri;
            }
            else
            {
                this.Url = $"{this.Version}/{this.Organization}/" + this.Url;
            }
            
            var response = this.InvokeRestMethod(this.Url, this.Body, this.Method).Result;
            if (string.IsNullOrEmpty(this.OutFile))
            {
                var result = response.Content;
                if (result != null)
                {
                    this.Logger.WriteObject(result);
                }
            }
            else
            {
                using (var fileStream = new FileStream(this.OutFile, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (var responseStream = response.ContentStream)
                    {
                        responseStream.CopyTo(fileStream);
                    }
                }

                this.Logger.WriteVerbose($"OutFile '{this.OutFile}' created");
            }
        }



        private async Task<HttpResult> InvokeRestMethod(string url, string body, PowerBIWebRequestMethod requestType)
        {
            // https://msdn.microsoft.com/en-us/library/mt243842.aspx
            // https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
            var token = this.Authenticator.Authenticate(this.Profile, this.Logger, this.Settings);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.Profile.Environment.GlobalServiceEndpoint);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

                HttpResponseMessage response = null;
                if (string.IsNullOrEmpty(this.OutFile))
                {
                    switch (requestType)
                    {
                        case PowerBIWebRequestMethod.Get:
                            response = await client.GetAsync(url);
                            break;
                        case PowerBIWebRequestMethod.Post:
                            response = await client.PostAsync(url, new StringContent(body));
                            break;
                        case PowerBIWebRequestMethod.Delete:
                            response = await client.DeleteAsync(url);
                            break;
                        case PowerBIWebRequestMethod.Put:
                            response = await client.PutAsync(url, new StringContent(body));
                            break;
                        case PowerBIWebRequestMethod.Patch:
                            response = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = new StringContent(body) });
                            break;
                        case PowerBIWebRequestMethod.Options:
                            response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Options, url));
                            break;
                        default:
                            throw new NotSupportedException($"{nameof(requestType)} of value {requestType} is not supported");
                    }
                }
                else
                {
                    // Stream based, OutFile
                    HttpRequestMessage request = null;
                    switch (requestType)
                    {
                        case PowerBIWebRequestMethod.Get:
                            request = new HttpRequestMessage(HttpMethod.Get, url);
                            break;
                        case PowerBIWebRequestMethod.Post:
                            request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(body) };
                            break;
                        case PowerBIWebRequestMethod.Delete:
                            request = new HttpRequestMessage(HttpMethod.Delete, url);
                            break;
                        case PowerBIWebRequestMethod.Put:
                            request = new HttpRequestMessage(HttpMethod.Put, url) { Content = new StringContent(body) };
                            break;
                        case PowerBIWebRequestMethod.Patch:
                            request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = new StringContent(body) };
                            break;
                        case PowerBIWebRequestMethod.Options:
                            request = new HttpRequestMessage(HttpMethod.Options, url);
                            break;
                        default:
                            throw new NotSupportedException($"{nameof(requestType)} of value {requestType} is not supported");
                    }

                    response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                }

                this.Logger.WriteVerbose($"Request Uri: {response.RequestMessage.RequestUri}");
                this.Logger.WriteVerbose($"Status Code: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var result = new HttpResult()
                {
                    ResponseMessage = response
                };

                if (string.IsNullOrEmpty(this.OutFile))
                {
                    result.Content = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    result.ContentStream = memoryStream;
                }

                return result;
            }
        }

        public class HttpResult
        {
            public HttpResponseMessage ResponseMessage { get; set; }
            public string Content { get; set; }
            public Stream ContentStream { get; set; }
        }
    }
}
