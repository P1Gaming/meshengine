using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using MeshEngine.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MockTestBinarySaving : MonoBehaviour
{
    string savePath;
    string fileName = "testSave.bin";
    string fullPath => savePath + fileName;
    // Start is called before the first frame update
    void Start()
    {
        savePath = Application.persistentDataPath + "/";

        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        bool createNewFile = !File.Exists(fullPath);


        if (createNewFile)
        {
            using (Stream stream = new FileStream(fullPath, FileMode.Create, FileAccess.ReadWrite))
            {
                
                byte[] buffer = new byte[4];

                buffer[0] = 2;
                buffer[1] = 9;
                buffer[2] = 255;
                buffer[3] = 15;

                foreach (byte b in buffer)
                {
                    Debug.Log(b);
                }
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(buffer);
                }

                stream.Close();
            }
        }
    }
    public void ReadBinFile()
    {
        using (Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite))
        {stream.Seek(0, SeekOrigin.Begin);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            byte[] results = new byte[8];

            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] bytes = reader.ReadBytes(8);

                foreach (byte b in bytes)
                {
                    Debug.Log(b);
                }
                

            }
                

            foreach (byte b in results)
            {
                Debug.Log(b);
            }
            
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
