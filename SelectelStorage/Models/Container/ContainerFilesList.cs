using System.Collections.Generic;
using SelectelStorage.Models.File;

namespace SelectelStorage.Models.Container
{
    /// <summary>
    /// Список контейнеров, так же содержащий информацию о хранилище
    /// </summary>
    public class ContainerFilesList : List<FileInfo>
    {
        /// <summary>
        /// Информация о хранилище
        /// </summary>
        public StorageInfo StorageInfo { get; internal set; }
    }
}
