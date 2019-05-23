using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lotto.Startup))]
namespace Lotto
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
