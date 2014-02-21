using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureTagger
{
    [Serializable]
    public class ImageFile
    {
        public IList<string> Tags { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public bool Exists { get; set; }
        public string FullPath { get; set; }

        public ImageFile(string fullPath)
        {
            //parse file name
            FileName =
              fullPath.Substring(fullPath.LastIndexOf("\\") + 1);
            Exists = true;
            FullPath = fullPath;
            Tags = new List<string>();
        }

    }
}
