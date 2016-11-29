namespace msiotCommunitySamples
{
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Session;
    using Nancy.TinyIoc;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        //https://blog.csainty.com/2012/05/enabling-sessions-in-nancy.html
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            CookieBasedSessions.Enable(pipelines,Nancy.Cryptography.CryptographyConfiguration.Default);
        }
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper


    }
}