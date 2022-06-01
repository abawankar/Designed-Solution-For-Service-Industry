using System.Collections.Generic;
using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class NatureGroup : INatureGroup
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<IBusinessNature> NatureName { get; set; }
    }
}
