using Dolittle.Runtime.Events.SqlLite.Specs;

namespace Dolittle.Runtime.Events.SqlLite.Relativity.Specs
{
    using Dolittle.Runtime.Events.Relativity;
    using Dolittle.Runtime.Events.SqlLite.Relativity;
    using Dolittle.Runtime.Events.Relativity.Specs;
    public class SUTProvider : IProvideGeodesics
    {
        public IGeodesics Build() => new test_geodesics(new a_database());
    }
}