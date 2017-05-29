using System;
using System.ServiceModel;
using Common.ServiceContract;

namespace PriceList.Service
{
    public class PriceListService
    {
        private static PriceListService service;
        private readonly ChannelFactory<IPriceList> factory;

        private PriceListService()
        {
            Uri address = new Uri("http://IT-1:4000/PriceList");
            BasicHttpBinding binding = new BasicHttpBinding();
            EndpointAddress endpoint = new EndpointAddress(address);

            factory = new ChannelFactory<IPriceList>(binding, endpoint);
            //IPriceList channel = factory.CreateChannel();
        }

        public static ChannelFactory<IPriceList> GetFactory()
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
