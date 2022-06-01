using System.Collections.Generic;

namespace Domain.Interface.Master
{
    public interface INatureGroup
    {
        int Id { get; set; }
        string Name { get; set; }
        IList<IBusinessNature> NatureName { get; set; }
    }
}
