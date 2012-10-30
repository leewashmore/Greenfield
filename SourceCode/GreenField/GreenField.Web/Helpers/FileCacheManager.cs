using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Caches the content in a txt file on a file server
    /// </summary>
    class FileCacheManager
    {
        /// <summary>
        /// Name of the cache file i.e. Cahce28102012
        /// </summary>
        public string cacheFileName
        {
            get
            {
                return "Cache" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            }
        }

        /// <summary>
        /// Folder Path where cache file would reside
        /// </summary>
        public string DirectoryPath { get; private set; }

        /// <summary>
        /// Cache File Path
        /// </summary>
        public string CacheFilePath
        {
            get
            {
                return DirectoryPath + @"\" + cacheFileName;
            }
        }

        public FileCacheManager(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        /// <summary>
        /// Add Item to the Cache File. Key is seperated from value by || and value ends with @@
        /// appended at the end
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        public void SetCacheItem(string key, string value)
        {
            bool cacheFileExists = false;

            //Check if cache file exists
            if (!File.Exists(CacheFilePath))
            {
                cacheFileExists = CreateCacheFile(CacheFilePath);
            }
            else
            {
                cacheFileExists = true;
            }

            AddItemToFile(CacheFilePath, key, value);
        }

        /// <summary>
        /// Get Value for the corresponding from cache file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCacheItem(string key)
        {
            string cacheValue = String.Empty;

            cacheValue = GetItemFromFile(CacheFilePath, key);

            return cacheValue;
        }

        private string GetItemFromFile(string cacheFilePath, string key)
        {
            string cacheValue = string.Empty;
            FileStream cacheFile = null;
            StreamReader sr = null;
            try
            {
                if (File.Exists(cacheFilePath))
                {
                    cacheFile = new FileStream(cacheFilePath, FileMode.Open, FileAccess.Read);
                    sr = new StreamReader(cacheFile);
                    while (sr.Peek() >= 0)
                    {
                        string content = sr.ReadLine();
                        string contentKey = content.Substring(0, content.IndexOf("||"));

                        if (contentKey.Equals(key, StringComparison.InvariantCultureIgnoreCase))
                        {
                            int startIndex = content.IndexOf("||") + 2;
                            int length = content.IndexOf("@@") - startIndex;
                            cacheValue = content.Substring(startIndex, length);
                        }
                    }
                }

                return cacheValue;
            }
            finally
            {
                if (sr != null)
                {
                    //sr.Close();
                    sr.Dispose();
                }
                if (cacheFile != null)
                {
                    cacheFile.Close();
                    cacheFile = null;
                }
            }
        }

        private void AddItemToFile(string cacheFilePath, string key, string value)
        {
            FileStream cacheFile = null;
            StreamWriter sw = null;

            try
            {
                string text = key + @"||" + value + @"@@";
                cacheFile = new FileStream(cacheFilePath, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(cacheFile);
                sw.Write(text);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (cacheFile != null)
                {
                    cacheFile.Close();
                    cacheFile = null;
                }
            }
        }

        private bool CreateCacheFile(string cacheFilePath)
        {
            try
            {
                if (!String.IsNullOrEmpty(cacheFilePath) && Directory.Exists(Path.GetDirectoryName(cacheFilePath)))
                {
                    if (!File.Exists(cacheFilePath))
                    {
                        File.Create(cacheFilePath);
                        return true;
                    }
                }

                return false;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Value for the Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public String this[string key]
        {
            get
            {
                return GetCacheItem(key);
            }
        }
    }
}