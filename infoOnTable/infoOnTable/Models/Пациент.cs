//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace infoOnTable.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Пациент
    {
        public int Id_patient { get; set; }
        public int id_doc { get; set; }
        public string Фамилия { get; set; }
        public Nullable<int> Номер_талона { get; set; }
    
        public virtual Врач Врач { get; set; }
    }
}
