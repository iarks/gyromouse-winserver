using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GyroMouseServer;
using System.Collections.Concurrent;
using System.Threading;

namespace GyroMouseServer
{
    class ClientRequestHandler
    {
        BlockingCollection<ClientRequest> requestQueue;

        public ClientRequestHandler(BlockingCollection<ClientRequest> requestQueue)
        {
            this.requestQueue = requestQueue;
        }

        public void handleRequests()
        {
            while(true)
            {
                
            }
        }
    }
}
