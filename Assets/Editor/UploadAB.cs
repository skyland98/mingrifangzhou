using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UploadAB 
{
    //[MenuItem("AB������/�ϴ�AB���ͶԱ��ļ�")]
    public static void UploadAllABFiles()
    {
        DirectoryInfo directory = Directory.CreateDirectory("E:\\VR\\AR\\AssetBundles\\PC\\");
        FileInfo[] fileInfos = directory.GetFiles();


        foreach (FileInfo info in fileInfos)
        {
            //û�к�׺�Ĳ���AB��
            if (info.Extension == "" || info.Extension == ".txt")
            {
                FtpUploadFiles(info.FullName, info.Name);
            }
        }
    }


    public async static void FtpUploadFiles(string filePath,string fileName)
    {
        await Task.Run(() => {
            try
            {
                FtpWebRequest req = FtpWebRequest.Create(new Uri("ftp://192.168.124.5/AB/PC/" + fileName)) as FtpWebRequest;
                NetworkCredential n = new NetworkCredential("Liu", "Liu123");
                req.Credentials = n;

                req.Proxy = null;
                req.KeepAlive = false;
                req.Method = WebRequestMethods.Ftp.UploadFile;
                req.UseBinary = true;

                Stream upLoadStream = req.GetRequestStream();
                using (FileStream file = File.OpenRead(filePath))
                {
                    //һ��һ���ϴ�
                    byte[] bytes = new byte[2048];

                    int contentLength = file.Read(bytes, 0, bytes.Length);

                    while (contentLength != 0)
                    {
                        upLoadStream.Write(bytes, 0, contentLength);
                        contentLength = file.Read(bytes, 0, bytes.Length);
                    }

                    file.Close();
                    upLoadStream.Close();
                }
                Debug.Log(fileName + "�ϴ��ɹ�");
            }
            catch (Exception e)
            {
                Debug.Log(fileName + "�ϴ�ʧ��" + e.Message);
            }
        });       
    }
}
