using Domain.Implementation.Master;
using FluentNHibernate.Mapping;

namespace Domain.Mapping.Master
{
    public class EnquirySourceMap : ClassMap<EnquirySource>
    {
        public EnquirySourceMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.AppointmentDate);
            Map(x => x.TerminationDate);
            Map(x => x.RetainerFee);
            Map(x => x.CommLeisure);
            Map(x => x.CommMice);
            Map(x => x.Active);
        }
    }
}
