using Newtonsoft.Json;

namespace UnicoStudio.UnicoLibs.Utilities
{
    public static class SerializationHelper
    {
        static JsonSerializerSettings populateObjectSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Reuse
        };

        public static T Deserialize<T>(string dataToDeserialize, bool decrypt = true)
        {
            if (string.IsNullOrEmpty(dataToDeserialize))
                throw new System.Exception($"{nameof(dataToDeserialize)} cannot be null or empty!");

            string data;

            if (decrypt)
            {
                string decryptedData = Crypto.Decrypt(dataToDeserialize);
                data = GZip.DecompressData(decryptedData);
            }
            else
            {
                data = dataToDeserialize;
            }

            T deserializedData = JsonConvert.DeserializeObject<T>(data);

            return deserializedData;
        }

        public static void DeserializeInto(string dataToDeserialize, object objectToLoadInto, bool decrypt = true)
        {
            if (string.IsNullOrEmpty(dataToDeserialize))
                return;

            string data;

            if (decrypt)
            {
                string decryptedData = Crypto.Decrypt(dataToDeserialize);
                data = GZip.DecompressData(decryptedData);
            }
            else
            {
                data = dataToDeserialize;
            }

            JsonConvert.PopulateObject(data, objectToLoadInto, populateObjectSettings);
        }

        public static string Serialize(object objectToSerialize, bool encrypt = true)
        {
            if (objectToSerialize == null)
                return null;

            string serializedData = SerializeAsJson(objectToSerialize);

            if (!encrypt)
                return serializedData;

            string encryptedData = CompressAndEncryptData(serializedData);

            return encryptedData;
        }

        public static string CompressAndEncryptData(string serializedData)
        {
            if (string.IsNullOrEmpty(serializedData))
                return null;

            string compressedData = GZip.CompressData(serializedData);
            string encryptedData = Crypto.Encrypt(compressedData);

            return encryptedData;
        }

        public static string SerializeAsJson(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }

        public static void CloneObject(object objectToFill, object objectToBeClonned)
        {
            if (objectToBeClonned == null)
                return;

            string serializedData = SerializeAsJson(objectToBeClonned);
            JsonConvert.PopulateObject(serializedData, objectToFill, populateObjectSettings);
        }
    }

    public static class SerializationExtensions
    {
        public static string Serialize(this object value)
        {
            return SerializationHelper.Serialize(value);
        }

        public static T Deserialize<T>(this string data)
        {
            return SerializationHelper.Deserialize<T>(data);
        }
    }
}