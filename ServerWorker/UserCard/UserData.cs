using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Interfaces;

namespace ServerWorker.UserCard
{
    public class UserData
    {
        
        public DateTime timeLastUpdateData;

        public string id;
        public ISetting setting = new ISetting();
        public IInfoDevice infoDevice = new IInfoDevice();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public ClientLog clientLog;

        public static string parthUserCard = "UserCards";


        public void SaveDataToFile(string filename)
        {
            if (!Directory.Exists(parthUserCard))
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
