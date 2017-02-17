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
using websocket_chat_application.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;


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
            shortenURLEntities dc = new shortenURLEntities();

            private static WebSocketCollection _chatClients = new WebSocketCollection();
            private string _userid;

            //     public ChatWebSocketHandler(string username)
            //    {
            //         _username = username;
            //     }

            //     public override void OnOpen()
            //     {
            //         _chatClients.Add(this);
            //     }

            //     public override void OnMessage(string message)
            //     {
            //        _chatClients.Broadcast(_username + ": " + message);
            //     }
            //}

            public ChatWebSocketHandler(string userid)
            {
                _userid = userid;
            }

            public override void OnOpen()
            {
                _chatClients.Add(this);
                var res = getdata(_userid);
                _chatClients.Broadcast("result: " + res);
            }

            //public override void OnMessage(string message)
            //{
            //   _chatClients.Broadcast(_username + ": " + message);
            //}
            public class appUrlModel
            {
                public string admin { get; set; }
                public string analytics { get; set; }
                public string landing { get; set; }
            }

            public DashBoardSummary getdata(string userid)
            {
                int c_id = Convert.ToInt32(userid);
                DashBoardSummary obj = new DashBoardSummary();
                SqlConnection lSQLConn = null;
                SqlCommand lSQLCmd = new SqlCommand();
                Client obj_client = dc.Clients.Where(x => x.PK_ClientID == c_id).Select(y => y).SingleOrDefault();
                if (obj_client != null)
                {
                    string connStr = ConfigurationManager.ConnectionStrings["shortenURLConnectionString"].ConnectionString;

                    // create and open a connection object
                    lSQLConn = new SqlConnection(connStr);
                    SqlDataReader myReader;
                    lSQLConn.Open();
                    lSQLCmd.CommandType = CommandType.StoredProcedure;
                    lSQLCmd.CommandText = "spGetDashBoardSummary";
                    lSQLCmd.Parameters.Add(new SqlParameter("@FkClientId", c_id));
                    lSQLCmd.Connection = lSQLConn;
                    myReader = lSQLCmd.ExecuteReader();

                    totalUrls totalUrls = ((IObjectContextAdapter)dc)
                      .ObjectContext
                      .Translate<totalUrls>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    // Move to locations result 
                    myReader.NextResult();
                    users users = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<users>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    // Move to locations result 
                    myReader.NextResult();
                    visits visits = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<visits>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    // Move to locations result 
                    myReader.NextResult();
                    campaigns campaigns = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<campaigns>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    // Move to locations result 
                    myReader.NextResult();
                    List<recentCampaigns1> recentCampaigns = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<recentCampaigns1>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).ToList();

                    // Move to locations result 
                    myReader.NextResult();
                    today today = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<today>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    // Move to locations result 
                    myReader.NextResult();
                    last7days last7days = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<last7days>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    // Move to locations result 
                    myReader.NextResult();
                    month month = ((IObjectContextAdapter)dc)
                   .ObjectContext
                   .Translate<month>(myReader, "SHORTURLDATAs", MergeOption.AppendOnly).SingleOrDefault();

                    List<recentCampaigns> objr = (from r in recentCampaigns
                                                  select new recentCampaigns()
                                                  {
                                                      id = r.id,
                                                      rid = r.rid,
                                                      visits = r.visits,
                                                      users = r.users,
                                                      status = r.status,
                                                      //crd = r.createdOn.Value.ToString("MM/dd/yyyyThh:mm:ss")
                                                      createdOn = r.crd.Value.ToString("yyyy-MM-ddThh:mm:ss"),
                                                      endDate = (r.endd == null) ? null : (r.endd.Value.ToString("yyyy-MM-ddThh:mm:ss"))

                                                  }).ToList();

                    activities obj_act = new activities();
                    obj.totalUrls = totalUrls;
                    obj.users = users;
                    obj.visits = visits;
                    obj.campaigns = campaigns;
                    obj.recentCampaigns = objr;
                    obj_act.today = today;
                    obj_act.last7days = last7days;
                    obj_act.month = month;
                    obj.activities = obj_act;

                }
                return obj;

            }



        }
         
    }
}