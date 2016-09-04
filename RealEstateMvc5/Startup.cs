using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RealEstateMvc5.Startup))]
namespace RealEstateMvc5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
