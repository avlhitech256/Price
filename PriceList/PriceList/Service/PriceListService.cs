using System;
using System.ServiceModel;
using Common.ServiceContract;

namespace PriceList.Service
{
    public class PriceListService
    {
        private static PriceListService service;
        private readonly ChannelFactory<IPriceListService> factory;

        private PriceListService()
        {
            Uri address = new Uri("http://IT-1:4000/PriceList");
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(address);

            factory = new ChannelFactory<IPriceListService>(binding, endpoint);
            //IPriceList channel = factory.CreateChannel();
        }

        public static ChannelFactory<IPriceListService> GetFactory()
        {
            if (service == null)
            {
                service = new PriceListService();
            }

            return service.factory;
        }

        //static public IPriceList GetChannel()
        //{
        //    return factory.CreateChannel();
        //}
    }
}
