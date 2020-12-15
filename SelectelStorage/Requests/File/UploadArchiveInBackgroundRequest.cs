﻿using System;
using System.Collections.Specialized;
using System.Net;
using SelectelStorage.Models.File;

namespace SelectelStorage.Requests.File
{
    public class UploadArchiveInBackgroundRequest : BaseRequest<UploadArchiveResult>
    {
        private string Path { get; set; }

        public UploadArchiveInBackgroundRequest(
            byte[] file,            
            FileArchiveFormat archiveFormat,
            string path = null) : base()
        {
            SetArchiveFormat(archiveFormat);
            
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

            TryAddQueryParam("extract-archive-v2", format);
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
                this.Result = new UploadArchiveResult(headers);
            }
            else
            {
                ParseError(null, status);
            }
        }

        protected override string GetUrl(string storageUrl)
        {
            return string.Concat("https://api.selcdn.ru/v1/SEL_", "182815/", this.Path);
            //SEL_XXX/[имя контейнера]/?extract-archive-v2=tar.bz2' 
            //var url = storageUrl;
            //if (this.Path != null) {
            //    url = string.Concat(url, this.Path);
            //}

            //return url;
        }
    }
}
