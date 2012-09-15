using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Threading;
using System.Windows.Browser;
using Aspect;
using System.Text;

namespace Aspect
{
    public class PostRequest<T>
    {        
        private Object send_obj;

        private Uri RequestUrl;
        private Dispatcher Dispatcher;

        public delegate void ProcessResponseEvent(T response);
        public event ProcessResponseEvent ProcessResponse;

        public delegate void ProcessErrorEvent();
        public event ProcessErrorEvent ProcessError;

        public PostRequest(Dispatcher Dispatcher, string RequestUrl)
        {
            this.RequestUrl = new Uri(HtmlPage.Document.DocumentUri, RequestUrl);
            this.Dispatcher = Dispatcher;
        }

        public void Perform(Object data)
        {
            send_obj = data;
            WebRequest request = WebRequest.Create(this.RequestUrl);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.BeginGetRequestStream(new AsyncCallback(RequestReady), request);
        }

        // Sumbit the Post Data  
        void RequestReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request = asyncResult.AsyncState as HttpWebRequest;
            Stream stream = request.EndGetRequestStream(asyncResult);           

            this.Dispatcher.BeginInvoke(delegate()
            {         
                if (this.send_obj is string)
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.WriteLine(this.send_obj);
                    writer.Flush();
                    writer.Close();
                }
                else
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(send_obj.GetType());
                    serializer.WriteObject(stream, this.send_obj);
                    stream.Flush();
                    stream.Close();
                }
                request.BeginGetResponse(new AsyncCallback(ResponseReady), request);               
            });
        }

        public class PostJsonAnswer
        {
            public string d { set; get; }
        }

        // Get the Result  
        void ResponseReady(IAsyncResult asyncResult)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            // handler for "500 Interal Error"
            try
            {
                 request = asyncResult.AsyncState as HttpWebRequest;
                 response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            }
            catch
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    if (this.ProcessError != null)
                    {
                        this.ProcessError();
                    }
                });
                return;
            }

            this.Dispatcher.BeginInvoke(delegate()
            {                                  
                Stream responseStream = response.GetResponseStream();

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PostJsonAnswer));
                string raw_answer = ((PostJsonAnswer)serializer.ReadObject(responseStream)).d;

                serializer = new DataContractJsonSerializer(typeof(T));

                T resp_obj = (T)serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(raw_answer)));
                if (this.ProcessResponse != null)
                {
                    this.ProcessResponse(resp_obj);
                }
            });
        }
    }
}
