using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemCleaner
{
    public class File
    {
        public int file_id { get; set; }
        public string path { get; set; }

        public File(int file_id, string path)
        {
            this.file_id = file_id;
            this.path = path;
        }
    }
}
