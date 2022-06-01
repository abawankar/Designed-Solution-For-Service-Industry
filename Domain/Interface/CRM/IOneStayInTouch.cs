using System;

namespace Domain.Interface.CRM
{
    public interface IOneStayInTouch
    {

        int Id { get; set; }
        IContact Contact { get; set; }
        string CC { get; set; }
        string BCC { get; set; }
        string Subject { get; set; }
        string Notes { get; set; }
        string Signature { get; set; }
        DateTime Date { get; set; }
    }
}
