
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class LoginInfo : ILoginInfo
    {
        public virtual int Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual DateTime Date { get; set; }
    }
}
