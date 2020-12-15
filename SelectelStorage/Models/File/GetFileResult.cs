using System.Collections.Specialized;
using SelectelStorage.Headers;

namespace SelectelStorage.Models.File
{
    public class GetFileResult : FileInfo
    {                
        public byte[] File { get; set; }

        public GetFileResult(byte[] file, string name, NameValueCollection headers)
        {
            HeaderParsers.ParseHeaders(this, headers);
            this.File = file;
            this.Name = name;
        }
    }
}
