﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using SelectelStorage.Headers;
using SelectelStorage.Models.File;

namespace SelectelStorage.Requests.File
{
    public class UploadFileRequest : FileRequest<UploadFileResult>
    {
        private string ETag { get; set; }

        public UploadFileRequest(
            string containerName,
            string fileName,
            byte[] file,
            bool validateChecksum = false,
            string contentDisposition = null,
            string contentType = null,            
            long? deleteAt = null,
            long? deleteAfter = null,
            IDictionary<string, object> customHeaders = null) : base(containerName, fileName)
        {
            TryAddHeader(HeaderKeys.ContentDisposition, contentDisposition);

            if (deleteAfter.HasValue)
            {
                TryAddHeader(HeaderKeys.XDeleteAfter, deleteAfter.Value);
            }

            if (deleteAt.HasValue)
            {
                TryAddHeader(HeaderKeys.XDeleteAt, deleteAt.Value);
            }

            if (string.IsNullOrEmpty(contentType) == false)
            {
                TryAddHeader(HeaderKeys.ContentType, contentType);
            }

            if (string.IsNullOrEmpty(contentDisposition) == false)
            {
                TryAddHeader(HeaderKeys.ContentDisposition, contentDisposition);
            }

            SetCustomHeaders(customHeaders);

            if (validateChecksum)
            {
                this.ETag = GetETag(file);
                TryAddHeader(HeaderKeys.ETag, this.ETag);
            }

            this.File = file;
        }

        internal override RequestMethod Method
        {
            get
            {
                return RequestMethod.PUT;
            }
        }

        internal override void Parse(NameValueCollection headers, object data, HttpStatusCode status)
        {
            if (status == HttpStatusCode.Created)
            {
                if (this.ETag != null)
                {
                    // idk why Selectel's ETag check not working, so check the result once again on client.
                    if (headers[HeaderKeys.ETag].Equals(this.ETag, StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        this.Result = UploadFileResult.CheckSumValidationFailed;
                        return;
                    }
                }

                this.Result = UploadFileResult.Created;
            }
            else if ((int)status == 422)
            {
                this.Result = UploadFileResult.CheckSumValidationFailed;
            }
            else
            {
                ParseError(null, status);
            }
        }

        private string GetETag(byte[] file)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(file);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                //return Encoding.Default.GetString();
            }
        }
    }
}
