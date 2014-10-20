using Android.Content;
using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace DailySocial.Utils
{
    public static class IsoStorage
    {
        public static void Save<T>(string fileName, T item)
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
                using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.CreateNew, storage))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        writer.Write(item);
                    }

                    //var serializer = new DataContractSerializer(typeof(T));
                    //serializer.WriteObject(fileStream, item);
                    //fileStream.Close();
                }

                //storage.Close();
            }
        }

        public static string Load(string fileName)
        {
            try
            {
                using (var storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!storage.FileExists(fileName)) return "";
                    using (var fileStream = new IsolatedStorageFileStream(fileName, FileMode.Open, storage))
                    {
                        //var serializer = new DataContractSerializer(typeof(T));
                        //return (T)serializer.ReadObject(fileStream);
                        using (var reader = new StreamReader(fileStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void SaveNew(string fileName, string item)
        {
            using (var o = new StreamWriter(MyApplication.StaticContext.OpenFileOutput(fileName, FileCreationMode.Private)))
            {
                o.Write(item);
            }
        }

        public static string LoadNew(string fileName)
        {
            try
            {
                using (var i = new StreamReader(MyApplication.StaticContext.OpenFileInput(fileName)))
                {
                    return i.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}