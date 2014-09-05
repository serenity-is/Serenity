
namespace Serenity.Testing
{
    using System;

    public class MunqContext : IDisposable
    {
        private IDependencyResolver old;
        private IDependencyResolver mine;

        public MunqContext()
        {
            var mq = new MunqContainer();
            mine = mq;
            old = Dependency.SetResolver(mine);
        }

        public virtual void Dispose()
        {
            if (mine == null)
                return;

            if (Dependency.HasResolver && Dependency.Resolver != mine)
                throw new InvalidOperationException("Container changed! Possible multi-thread error, or disposing order mistake!");

            Dependency.SetResolver(old);

            ((MunqContainer)mine).Dispose();
            mine = null;
        }
    }
}