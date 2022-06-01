using System;

namespace Domain.Interface.Transaction
{
    public interface INBOComments
    {
        int Id { get; set; }
        string UserName { get; set; }
        DateTime Date { get; set; }
        string Comments { get; set; }
        INBO NBO { get; set; }
    }
}
