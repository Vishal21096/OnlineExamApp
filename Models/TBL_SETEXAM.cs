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

    public partial class TBL_SETEXAM
    {
        public int EXAM_ID { get; set; }
        public Nullable<System.DateTime> EXAM_DATE { get; set; }
        public Nullable<int> EXAM_FK_STU { get; set; }
        [Display(Name = "Exam Name")]
        [Required(ErrorMessage = "*This Field Is Required.")]
        public string EXAM_NAME { get; set; }
        public Nullable<int> EXAM_STD_SCORE { get; set; }
    
        public virtual TBL_STUDENT TBL_STUDENT { get; set; }
    }
}