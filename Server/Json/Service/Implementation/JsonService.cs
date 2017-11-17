using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Json.Contract;

namespace Json.Service.Implementation
{
    public class JsonService : IJsonService
    {
        #region Methods

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

        public PriceList ConvertToPriceList(MemoryStream stream)
        {
            PriceList result = Convert<PriceList>(stream);
            return result;
        }

        public PriceList ConvertToPriceList(string json)
        {
            PriceList result = Convert<PriceList>(json);
            return result;
        }

        public T Convert<T>(MemoryStream stream) where T : class, new ()
        {
            T result = new T();

            if (stream != null)
            {
                stream.Position = 0;
                DataContractJsonSerializer serializator = new DataContractJsonSerializer(typeof(T));

                try
                {
                    result = serializator.ReadObject(stream) as T;
                }
                catch (Exception e)
                {
                    result = new T();
                }
            }

            return result;
        }

        public T Convert<T>(string json) where T : class, new ()
        {
            T result = new T();

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        result = Convert<T>(stream);
                        stream.Close();
                    }
                }
                catch (Exception)
                {
                    result = new T();
                }
            }

            return result;
        }

        #endregion
    }
}
