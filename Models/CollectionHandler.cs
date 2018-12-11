using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace culTAKU.Models
{
    public static class CollectionHandler
    {
        private static string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"./MyCollection/SavedCollection.bin";

        public static void SaveMyCollectionFile(AnimeCollection MyAnimeCollection)
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//MyCollection");
            Stream file;
            try
            {
                file = File.Create(filePath);
            }
            catch (System.IO.IOException e)
            {
                return;
            }
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(file, MyAnimeCollection);
        }

        public static AnimeCollection LoadMyCollectionFile()
        {
            if (File.Exists(filePath))
            {
                using (Stream file = File.OpenRead(filePath))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    try
                    {
                        return (AnimeCollection)deserializer.Deserialize(file);
                    }

                    catch(SerializationException e)
                    {
                        return null;
                    }
                }
            }
            return null;
        }
    }
}
