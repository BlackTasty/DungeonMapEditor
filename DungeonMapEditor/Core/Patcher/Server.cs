using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Patcher
{
    class Server
    {
        public Server(string url)
        {
            URL = url;
        }

        public string URL { get; set; }

        public bool IsAvailable
        {
            get
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                    request.AllowAutoRedirect = false;
                    request.Method = WebRequestMethods.Http.Head;
                    using (WebResponse response = request.GetResponse())
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
