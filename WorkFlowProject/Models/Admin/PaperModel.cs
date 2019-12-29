using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WorkFlowProject.Models.Admin
{
    public class PaperModel
    {
        [DisplayName("No#")]
        public int PaperId { get; set; }
        [DisplayName("Uploaded By")]
        public string Name { get; set; }
        [DisplayName("Class/Standard")]
        public string PaperStandard { get; set; }
        [DisplayName("Subject")]
        public string PaperSubject { get; set; }

        public Nullable<bool> PaperStatus { get; set; }
        [DisplayName("Paper File")]
        public string FilePath { get; set; }
        public string FileName { get; set; }

    }
}