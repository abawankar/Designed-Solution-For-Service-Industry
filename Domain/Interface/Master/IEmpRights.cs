using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Master
{ 
    public interface IEmpRights
    {
        int Id { get; set; }
        string Code { get; set; }
        string Area { get; set; }
        string MnuName { get; set; }
        string Operation { get; set; }
        string Description { get; set; }
    }
}
