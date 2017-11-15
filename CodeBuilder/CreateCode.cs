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
    class CreateCode
    {
        public static string CreateEntityClass(DbNewTable classInfo)
        {
            string templatePath = string.Empty;
            try
            {
                templatePath = System.Configuration.ConfigurationManager.AppSettings["TemplateEntity"].ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("读取配置文件错误！TemplateEntity" + ex.Message);
                return null;
            }
            if (!File.Exists(templatePath))
            {
                //MessageBox.Show("未找到Entity.tt，请修改配置文件！");
                return null;
            }
            CustomTextTemplatingEngineHost host = new CustomTextTemplatingEngineHost();
            host.TemplateFileValue = templatePath;
            string input = File.ReadAllText(templatePath);
            host.Session = new TextTemplatingSession();
            host.Session.Add("table", classInfo);

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
            return output;
        }

        public static string CreateDataAccessClass(DbNewTable classInfo)
        {
            string templatePath = string.Empty;
            try
            {
                templatePath = System.Configuration.ConfigurationManager.AppSettings["TemplateDataAccess"].ToString();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("读取配置文件错误！TemplateDataAccess" + ex.Message);
                return null;
            }
            if (!File.Exists(templatePath))
            {
                //MessageBox.Show("未找到DataAccess.tt，请修改配置文件！");
                return null;
            }
            CustomTextTemplatingEngineHost host = new CustomTextTemplatingEngineHost();
            host.TemplateFileValue = templatePath;
            string input = File.ReadAllText(templatePath);
            host.Session = new TextTemplatingSession();
            host.Session.Add("table", classInfo);

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

            return output;
        }
    }
}
