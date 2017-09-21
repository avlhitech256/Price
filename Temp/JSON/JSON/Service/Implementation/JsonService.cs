using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using JSON.Contract;

namespace JSON.Service.Implementation
{
    public class JsonService : IJsonService
    {
        public Clients ConvertToClients(MemoryStream stream)
        {
            Clients result = Convert<Clients>(stream);
            return result;
        }

        public Clients ConvertToClients(string json)
        {
            Clients result = Convert<Clients>(json);
            return result;
        }

        public MetaData ConvertToMetaData(MemoryStream stream)
        {
            MetaData result = Convert<MetaData>(stream);
            return result;
        }

        public MetaData ConvertToMetaData(string json)
        {
            MetaData result = Convert<MetaData>(json);
            return result;
        }

        public T Convert<T>(MemoryStream stream) where T : class
        {
            T result = null;

            if (stream != null)
            {
                stream.Position = 0;
                DataContractJsonSerializer serializator = new DataContractJsonSerializer(typeof(T));
                result = serializator.ReadObject(stream) as T;
            }

            return result;
        }

        public T Convert<T>(string json) where T : class 
        {
            T result = null;

            if (!string.IsNullOrWhiteSpace(json))
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    result = Convert<T>(stream);
                    stream.Close();
                }
            }

            return result;
        }
    }
}
