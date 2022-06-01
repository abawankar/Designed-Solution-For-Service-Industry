using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class Client : IClient
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICountry Country { get; set; }
        public virtual string ClientGroup { get; set; }
        public virtual string Remarks { get; set; }
    }
}
