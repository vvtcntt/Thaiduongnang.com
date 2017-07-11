using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(THAIDUONGNANG.Startup))]
namespace THAIDUONGNANG
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
