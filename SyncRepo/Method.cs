using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Xml;
using SharpSvn;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace SyncRepo
{
    public class Method
    {
        public static string GetInfo(string location)
        {
            string doCommand = string.Empty;
            string result = string.Empty;
            string serverPath = string.Empty;
            string localPath = string.Empty;
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe");

                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();

                process.StandardInput.WriteLine(@"cd " + location);
                process.StandardInput.WriteLine("svn info --xml > serverInfoTmp.xml");
                process.StandardInput.WriteLine("svn log -r head:0 --xml > localInfoTmp.xml");
                process.StandardInput.WriteLine("/C");
                process.WaitForExit();

                serverPath = location + "\\serverInfoTmp.xml";
                localPath = location + "\\localInfoTmp.xml";

                doCommand = CompareRevision(serverPath, localPath);
                result = ProcessCommand(doCommand, location);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        public static string CompareRevision(string serverPath, string localPath)
        {
            string command = string.Empty;
            List<string> lsServerLogRevision = new List<string>();
            List<string> lsCientLogRevision = new List<string>();

            #region server side
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(serverPath);

            XmlNodeList xmlServerNodeList = xmlDoc.SelectNodes("/info/entry/commit");
            foreach (XmlNode xn in xmlServerNodeList)
            {
                string logRevision = xn.Attributes[0].Value;
                lsServerLogRevision.Add(logRevision);
            }
            string serverRevision = lsServerLogRevision[0];
            #endregion
            #region client side
            xmlDoc.Load(localPath);

            XmlNodeList xmlClientNodeList = xmlDoc.SelectNodes("/log/logentry");
            foreach (XmlNode xn in xmlClientNodeList)
            {
                string logRevision = xn.Attributes[0].Value;
                lsCientLogRevision.Add(logRevision);
            }
            string clientRevision = lsCientLogRevision[0];
            #endregion

            if (Int32.Parse(serverRevision) > Int32.Parse(clientRevision))
                command = "update";
            else if (Int32.Parse(serverRevision) < Int32.Parse(clientRevision))
                command = "commit";
            else if (Int32.Parse(serverRevision) == Int32.Parse(clientRevision))
                command = "do nothing";

            return command;
        }

        public static string ProcessCommand(string command, string path)
        {
            string result = string.Empty;
            if (command == "update")
            {
                SvnUpdateResult svnResult;
                SvnUpdateArgs args = new SvnUpdateArgs();

                using (SvnClient client = new SvnClient())
                {
                    client.Update(path, args, out svnResult);
                }
                result = "updated";
            }
            else if (command == "commit")
            {
                SvnCommitArgs args = new SvnCommitArgs();
                args.LogMessage = "commited by system at " + DateTime.Now;

                using (SvnClient client = new SvnClient())
                {
                    client.Commit(path, args);
                }
                result = "commited";
            }
            else if (command == "do nothing")
            {
                result = "nothing changed";
            }

            return result;
        }

        public static bool CompareFile(string file1, string file2)
        {
            try
            {
                byte[] byte1 = File.ReadAllBytes(file1);
                byte[] byte2 = File.ReadAllBytes(file2);

                if (file1.Length == file2.Length)
                {
                    for (int i = 0; i < file1.Length; i++)
                    {
                        if (file1[i] != file2[i])
                        {
                            return false;

                        }
                    }
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            
        }
    }
}
