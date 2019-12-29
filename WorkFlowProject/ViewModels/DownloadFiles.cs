using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WorkFlowProject.Models.Admin;
using WorkFlowProject.Models.DataBaseModel;

namespace WorkFlowProject.ViewModels
{
    public class DownloadFiles
    {
        public List<PaperModel> GetFiles()
        {
            List<PaperModel> lstFiles = new List<PaperModel>();
            DirectoryInfo dirInfo = new DirectoryInfo(HostingEnvironment.MapPath("~/AppFiles/Files"));
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                lstFiles.Add(new PaperModel()
                {

                    PaperId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            return lstFiles;
        }
    }
}