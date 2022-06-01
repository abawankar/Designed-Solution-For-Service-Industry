using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interface.Master
{
    public interface ILoginInfo
    {
        int Id { get; set; }
        string UserName { get; set; }
        DateTime Date { get; set; }
    }
}
