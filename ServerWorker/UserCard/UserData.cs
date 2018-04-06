using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ServerWorker.UserCard
{
    public class UserData
    {
        
        public DateTime timeLastUpdateData;

        public string id;
        public int countStartProgram;
        public bool isMiner;
        public string dateFirstStart;
        public string timeFirstStart;
        public string startTime;
        public string version;
        public string name;

        public List<string> infoDevice = new List<string>();
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public ClientLog clientLog;
        public static string parthUserCard = "UserCards";


        public void SaveDataToFile(string filename)
        {
            if (Directory.Exists(parthUserCard))
            {
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(parthUserCard);
            }
            filename = parthUserCard +"\\"+ filename;
            XmlSerializer serializer = new XmlSerializer(typeof(UserData));
            TextWriter writer = new StreamWriter(filename);

            serializer.Serialize(writer, this);
            writer.Close();
        }
        public UserData RearDataFromFile(string filename)
        {
                XmlSerializer serializer = new XmlSerializer(typeof(UserData));
                serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
                serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);

                FileStream fs = new FileStream(filename, FileMode.Open);
                UserData result;
                result = (UserData)serializer.Deserialize(fs);
            fs.Close();
            return result;
        }
        private void Serializer_UnknownNode (object sender, XmlNodeEventArgs e)
        {
            Log.Send("UserData: Unknown Node:" + e.Name + "\t" + e.Text);
        }
        private void Serializer_UnknownAttribute (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            Log.Send("UserData: Unknown attribute " + attr.Name + "='" + attr.Value + "'");
        }

    }
}
