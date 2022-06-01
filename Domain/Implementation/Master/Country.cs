using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Country : ICountry
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
    }
}
