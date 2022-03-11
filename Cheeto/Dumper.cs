using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Cheeto
{
    [Serializable]
    public class SerializedEntry
    {
        public string Name = "";
        public List<string> Components = new List<string>();
        public List<SerializedEntry> Childrens = new List<SerializedEntry>();
    }

    class Dumper
    {
        public static string GetUniqueFileName(string name)
        {
            return name + "-" + Path.GetRandomFileName().Split('.')[0] + ".xml";
        }

        public static void DumpToFile(object o, string filename)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string directory = Path.Combine(folder, Application.productName);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string path = Path.Combine(directory, GetUniqueFileName(filename));

            SaveToFile(SerializeXml(o), path);
        }

        public static void SaveToFile(string text, string path)
        {
            File.WriteAllText(path, text);
        }

        public static string SerializeXml(object o)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer ser = new XmlSerializer(o.GetType());
            ser.Serialize(stream, o);
            stream.Seek(0, SeekOrigin.Begin);

            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            return text;
        }

        public static SerializedEntry SerializeScene()
        {
            SerializedEntry entry = new SerializedEntry();

            entry.Name = "root";

            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            //scene = gameObject.scene;

            var rootObjects = new Il2CppSystem.Collections.Generic.List<GameObject>(scene.rootCount);
            scene.GetRootGameObjects(rootObjects);

            foreach (var go in rootObjects)
            {
                entry.Childrens.Add(Serialize(go));
            }

            return entry;
        }

        public static SerializedEntry Serialize(GameObject go)
        {
            SerializedEntry e = new SerializedEntry();
            e.Name = go.name;

            foreach (Component c in go.GetComponents<Component>())
            {
                if (!c) continue;

                e.Components.Add(c.GetIl2CppType().Name);
            }

            for (int i = 0, length = go.transform.childCount; i < length; i++)
            {
                Transform child = go.transform.GetChild(i);

                e.Childrens.Add(Serialize(child.gameObject));
            }

            return e;
        }

        public static SerializedEntry StrinfigyActive()
        {
            SerializedEntry root = new SerializedEntry();
            root.Name = "root";

            // todo

            return root;
        }
    }
}
