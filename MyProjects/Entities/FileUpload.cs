﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public partial class FileUpload
    {
        public int Id { get; set; }
        public Nullable<int> OrderId { get; set; }
        public string OrderTitle { get; set; }
        public Nullable<int> UploadById { get; set; }
        public string UploadByName { get; set; }
        public Nullable<System.DateTime> DateUpload { get; set; }
        public Nullable<int> ModifiedById { get; set; }
        public string ModifiedByName { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<int> CountModified { get; set; }
        public Nullable<bool> Published { get; set; }
        public string FileName { get; set; }
        public string FileLink { get; set; }
        public string Filekind { get; set; }
        public Nullable<bool> IsImage { get; set; }
        public string Note { get; set; }

        public virtual User User { get; set; }
    }
}
