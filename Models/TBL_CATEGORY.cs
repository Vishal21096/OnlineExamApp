//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineQuizApp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TBL_CATEGORY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TBL_CATEGORY()
        {
            this.TBL_QUESTIONS = new HashSet<TBL_QUESTIONS>();
        }
    
        public int CAT_ID { get; set; }
        [Display(Name = "Ctegory Name")]
        [Required(ErrorMessage = "*This Field Is Required.")]
        public string CAT_NAME { get; set; }
        public Nullable<int> CAT_FK_ADMIN_ID { get; set; }
        [Display(Name = "Enter Room Number")]
        [Required(ErrorMessage = "*This Field Is Required.")]
        public string room_number { get; set; }
    
        public virtual TBL_ADMIN TBL_ADMIN { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TBL_QUESTIONS> TBL_QUESTIONS { get; set; }
    }
}
