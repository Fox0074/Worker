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
        public static Action OnDataUpdate = delegate { };
        //[XmlIgnoreAttribute]
        //public Action MinerStateUpdate = delegate { };
        public DateTime timeLastUpdateData;

        public string id;
        public ISetting setting = new ISetting();
        public IInfoDevice infoDevice = new IInfoDevice();

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public ClientLog clientLog;
        private bool m_IsWorkinMiner;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public bool IsWorkinMiner { get { return m_IsWorkinMiner; } set { m_IsWorkinMiner = value; OnDataUpdate.Invoke(); } }
        public static string parthUserCard = "UserCards";

        public UserData()
        { }
        public UserData (string id)
        {
            this.id = id;
            LoadUserData();
        }

        private void LoadUserData()
        {
            string[] dirs = Directory.GetFiles(UserCard.UserData.parthUserCard, "*.xml");
            foreach (string file in dirs)
            {
                if (file == UserCard.UserData.parthUserCard + @"\" + id + ".xml")
                {
                    UserData buf = RearDataFromFile(file);
                    setting = buf.setting;
                    infoDevice = buf.infoDevice;
                    break;
                }
            }
            OnDataUpdate.Invoke();
        }
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
        private UserData RearDataFromFile(string filename)
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
