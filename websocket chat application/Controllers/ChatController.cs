using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;
using Microsoft.Web.WebSockets;


namespace websocket_chat_application.Controllers
{
    public class ChatController : ApiController
    {
//        public HttpResponseMessage Get()
//        {
//            if (HttpContext.Current.IsWebSocketRequest)
//            {
//                HttpContext.Current.AcceptWebSocketRequest(ProcessWSChat);
//            }

//            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
//        }

//        private async Task ProcessWSChat(AspNetWebSocketContext context)
//        {
//            WebSocket socket = context.WebSocket;
//            while (true)
//            {
//                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
//                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
//                if (socket.State == WebSocketState.Open)
//                {
//                    string userMessage = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
//                    userMessage = "You sent: " + userMessage + " at " + DateTime.Now.ToLongTimeString();
//                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMessage));
//                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
//                }
//                else
//                {
//                    break;
//                }
//            }
//        }


//    }
//}

        public HttpResponseMessage Get(string username)
       {
           HttpContext.Current.AcceptWebSocketRequest(new ChatWebSocketHandler(username));
           return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
       }
     
       class ChatWebSocketHandler : WebSocketHandler
       {
           private static WebSocketCollection _chatClients = new WebSocketCollection();
           private string _username;
    
           public ChatWebSocketHandler(string username)
          {
               _username = username;
           }
   
           public override void OnOpen()
           {
               _chatClients.Add(this);
           }
   
           public override void OnMessage(string message)
           {
              _chatClients.Broadcast(_username + ": " + message);
           }
      }





         
    }
}