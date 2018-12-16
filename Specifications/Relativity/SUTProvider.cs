using Dolittle.Runtime.Events.Sqlite.Specs;

namespace Dolittle.Runtime.Events.Sqlite.Relativity.Specs
{
    using Dolittle.Runtime.Events.Relativity;
    using Dolittle.Runtime.Events.Sqlite.Relativity;
    using Dolittle.Runtime.Events.Relativity.Specs;
    public class SUTProvider : IProvideGeodesics
    {
        public IGeodesics Build() => new test_geodesics(new a_database());
    }
}