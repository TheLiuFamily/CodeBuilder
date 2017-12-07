using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.IO;
using System.CodeDom.Compiler;
using System.Configuration;


namespace CodeBuilder
{
    public class TemplateData
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    class CreateCode
    {

        public static List<TemplateData> CreateTemplateClass(DbNewTable classInfo)
        {
            string templatePath = string.Empty;
            try
            {
                templatePath = ConfigurationManager.AppSettings["TemplateFilePath"];
            }
            catch (Exception ex)
            {
                return null;
            }
            if (!Directory.Exists(templatePath))
            {
                return null;
            }

            CustomTextTemplatingEngineHost host = new CustomTextTemplatingEngineHost();
            host.TemplateFileValue = templatePath;
            host.Session = new TextTemplatingSession();
            host.Session.Add("table", classInfo);
            var list = new List<TemplateData>();
            for (int i = 0; ; i++)
            {
                var tt = ConfigurationManager.AppSettings["Template" + i];
                if (string.IsNullOrEmpty(tt))
                    break;
                string input = File.ReadAllText(templatePath + "\\" + tt + ".tt");
                string output = new Engine().ProcessTemplate(input, host);
                StringBuilder errorWarn = new StringBuilder();
                foreach (CompilerError error in host.Errors)
                {
                    errorWarn.Append(error.Line).Append(":").AppendLine(error.ErrorText);
                }
                if (!File.Exists("Error.log"))
                {
                    File.Create("Error.log");
                }
                File.WriteAllText("Error.log", errorWarn.ToString());
                list.Add(new TemplateData { Name = tt, Content = output });
            }
            return list;
        }

        public static List<TemplateData> GetTemplateData()
        {
            string templatePath = string.Empty;
            try
            {
                templatePath = ConfigurationManager.AppSettings["TemplateFilePath"];
            }
            catch (Exception ex)
            {
                return null;
            }
            if (!Directory.Exists(templatePath))
            {
                return null;
            }
            var list = new List<TemplateData>();
            for (int i = 0; ; i++)
            {
                var tt = ConfigurationManager.AppSettings["Template" + i];
                if (string.IsNullOrEmpty(tt))
                    break;
                string input = File.ReadAllText(templatePath + "\\" + tt + ".tt");
                list.Add(new TemplateData { Name = tt, Content = input });
            }
            return list;
        }
        public static List<TemplateData> SaveTemplateData(List<TemplateData> list)
        {
            string templatePath = string.Empty;
            try
            {
                templatePath = ConfigurationManager.AppSettings["TemplateFilePath"];
            }
            catch (Exception ex)
            {
                return null;
            }
            if (!Directory.Exists(templatePath))
            {
                return null;
            }
            for (int i = 0; i < list.Count; i++)
            {
                var tt = ConfigurationManager.AppSettings["Template" + i];
                var l = list.FirstOrDefault(q => q.Name == tt);
                if (l != null)
                {
                    File.WriteAllText(templatePath + "\\" + tt + ".tt", l.Content);
                }
            }
            return list;
        }
}
}
