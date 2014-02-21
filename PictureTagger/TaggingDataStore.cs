using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace PictureTagger
{
    [Serializable]
    public class TaggingDataStore
    {
        private static readonly string STORE_FILE_NAME = "\\TaggingDataStore.bin";
        private IDictionary<string, ImageFile> _images = null;
        private IEnumerable<string> _imageKeys = null;
        private string _currentKey = null;
        public IEnumerable<string> Tags { get { return _allTags; } }
        private IList<string> _allTags = new List<string>();

        public int ImageCount
        {
            get
            {
                return _images.Count;
            }
        }

        public ImageFile Next
        {
            get
            {
                _currentKey = _imageKeys.After(_currentKey);
                return CurrentImage;
            }
        }

        public ImageFile Previous
        {
            get
            {
                _currentKey = _imageKeys.Before(_currentKey);
                return CurrentImage;
            }
        }

        public ImageFile CurrentImage
        {
            get
            {
                return _images[_currentKey];
            }
        }

        private TaggingDataStore()
        {
            _images = new Dictionary<string, ImageFile>();
        }

        public static TaggingDataStore LoadFromFile(string directory)
        {
            string filePath = directory + STORE_FILE_NAME;
            TaggingDataStore store;
            if (!File.Exists(filePath))
            {
                store = new TaggingDataStore();
            }
            else
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                store = (TaggingDataStore)formatter.Deserialize(stream);
                stream.Close();
            }
            store.LoadFileList(directory);
            store._imageKeys = store._images.Keys;
            if (store._currentKey == null || !store._imageKeys.Contains(store._currentKey))
                store._currentKey = store._imageKeys.First();
            return store;
        }

        public void WriteToFile(string directory)
        {
            string filePath = directory + STORE_FILE_NAME;
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        private void LoadFileList(string directory)
        {
            foreach (string filePath in Directory.GetFiles(directory))
            {
                string pattern = @"\.(jpg|png|jpeg)$";
                if (Regex.IsMatch(filePath, pattern, RegexOptions.IgnoreCase))
                {
                    ImageFile imageFile = new ImageFile(filePath);
                    if (!_images.ContainsKey(imageFile.FileName))
                    {
                        _images.Add(imageFile.FileName, imageFile);
                    }
                    _images[imageFile.FileName].FullPath = filePath;
                }
            }
        }

        internal bool ContainsTag(string text)
        {
            return _allTags.Contains(text);
        }

        internal void AddTag(string text)
        {
            _allTags.Add(text);
        }

    }
}
