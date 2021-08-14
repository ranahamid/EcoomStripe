using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InjuryRiskAssessment.Startup))]
namespace InjuryRiskAssessment
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
