//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class EmailLog
    {
        public long Id { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public System.DateTime SentOn { get; set; }
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
    }
    
}