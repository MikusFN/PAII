using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Generic
{
    public static class Extension
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                T obj = (T) bf.Deserialize(memStream);
                return obj;
            }
        }
    }
}

