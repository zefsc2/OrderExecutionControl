//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrderExecutionControl
{
    using System;
    using System.Collections.Generic;
    
    public partial class commission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public commission()
        {
            this.subtask = new HashSet<subtask>();
        }
    
        public int code_commission { get; set; }
        public string name_commission { get; set; }
        public string text_commission { get; set; }
        public int code_status { get; set; }
        public Nullable<System.DateTime> date_of_registration_commssion { get; set; }
        public Nullable<System.DateTime> start_date_commission { get; set; }
        public Nullable<System.DateTime> end_date_commission { get; set; }
        public string attached_file { get; set; }
        public int code_employee { get; set; }
    
        public virtual employee employee { get; set; }
        public virtual status status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<subtask> subtask { get; set; }
    }
}
