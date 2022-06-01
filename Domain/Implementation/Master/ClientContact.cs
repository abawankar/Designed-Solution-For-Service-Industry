using Domain.Interface.Master;

namespace Domain.Implementation.Master
{
    public class ClientContact : IClientContact
    {
        public virtual int Id { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string EmailId { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string Fax { get; set; }
    }
}
