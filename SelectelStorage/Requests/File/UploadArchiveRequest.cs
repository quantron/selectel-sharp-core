﻿using System;
using System.Collections.Specialized;
using System.Net;
using SelectelStorage.Headers;
using SelectelStorage.Models.File;

namespace SelectelStorage.Requests.File
{
    public class UploadArchiveRequest : BaseRequest<UploadArchiveResult>
    {
        private string Path { get; set; }

        public UploadArchiveRequest(
            byte[] file,            
            FileArchiveFormat archiveFormat,
            string path = null) : base()
        {
            SetArchiveFormat(archiveFormat);
            TryAddHeader(HeaderKeys.Accept, HeaderKeys.AcceptJson);
            
            this.Path = path;
            this.File = file;
        }

        private void SetArchiveFormat(FileArchiveFormat archiveFormat)
        {
            string format;
            switch (archiveFormat)
            {
                case FileArchiveFormat.Tar:
                    format = "tar";
                    break;
                case FileArchiveFormat.TarGz:
                    format = "tar.gz";
                    break;
                case FileArchiveFormat.TarBz2:
                    format = "tar.bz2";
                    break;
                default:
                    throw new ArgumentException("Not supported archive format.");
            }

            TryAddQueryParam("extract-archive", format);
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
                base.Parse(headers, data, status);
            }
            else
            {
                ParseError(null, status);
            }
        }

        protected override string GetUrl(string storageUrl)
        {
            var url = storageUrl;
            if (this.Path != null) {
                url = string.Concat(url, this.Path);
            }

            return url;
        }
    }
}
