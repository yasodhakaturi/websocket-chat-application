using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(websocket_chat_application.Startup))]
namespace websocket_chat_application
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
