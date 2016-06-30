using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineDiary.Startup))]
namespace OnlineDiary
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
