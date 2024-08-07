using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using System;

public class ABTools : EditorWindow
{
    private int nowSelIndex = 0;
    private string[] targetStrings = new string[] { "PC","IOS","Android"};

    private string serverIP = "ftp://127.0.0.1";

    [MenuItem("AB������/�򿪹��ߴ���")]
    public static void OpenWindow()
    {
        ABTools windown = EditorWindow.GetWindowWithRect(typeof(ABTools),new Rect(0,0,350,220)) as ABTools;
        windown.Show();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 150, 15), "ƽ̨ѡ��");
        nowSelIndex = GUI.Toolbar(new Rect(10, 30, 250, 20),nowSelIndex,targetStrings);

        GUI.Label(new Rect(10, 60, 150, 15), "��Դ��������ַ");
        serverIP = GUI.TextField(new Rect(10, 80, 150, 20),serverIP);

        if(GUI.Button(new Rect(10, 110, 100, 40),"�����Ա��ļ�"))
        {
            CreateABCompareFile();
        }
        if (GUI.Button(new Rect(115, 110, 225, 40), "����Ĭ����Դ��StreamingAssets"))
        {
            MoveABToStreamingAssets();
        }
        if (GUI.Button(new Rect(10, 160, 330, 40), "�ϴ�AB���ͶԱ��ļ�"))
        {
            UploadAllABFiles();
        }
    }

    public void CreateABCompareFile()
    {
        DirectoryInfo directory = Directory.CreateDirectory("E:\\VR\\AR\\AssetBundles\\" + targetStrings[nowSelIndex]);
        FileInfo[] fileInfos = directory.GetFiles();

        //���ڴ洢��Ϣ�� �ַ���
        string abCompareInfo = "";

        foreach (FileInfo info in fileInfos)
        {
            //û�к�׺�Ĳ���AB��
            if (info.Extension == "")
            {
                //Debug.Log(info.Name);
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                abCompareInfo += "|";
            }
        }

        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);

        //Debug.Log(abCompareInfo);
        File.WriteAllText("E:\\VR\\AR\\AssetBundles\\"+ targetStrings[nowSelIndex] + "\\ABCompareInfo.txt", abCompareInfo);
        Debug.Log("AB���Ա��ļ����ɳɹ�");
    }

    public string GetMD5(string filePath)
    {

        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //�õ����ݵ�MD5�� 16���ֽ� ����
            byte[] md5Info = md5.ComputeHash(file);

            file.Close();

            //��16���ֽ�ת��Ϊ 16���� ƴ�ӳ��ַ��� Ϊ�˼���md5��ĳ���
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

    public void MoveABToStreamingAssets()
    {
        UnityEngine.Object[] selectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

        if (selectedAsset.Length == 0)
            return;

        //���ڴ洢��Ϣ�� �ַ���
        string abCompareInfo = "";

        foreach (UnityEngine.Object asset in selectedAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

            if (fileName.IndexOf(".") != -1)
                continue;

            AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);

            FileInfo info = new FileInfo(Application.streamingAssetsPath + fileName);
            abCompareInfo += info.Name + " " + info.Length + " " + CreateABCompare.GetMD5(info.FullName);
            abCompareInfo += "|";
        }
        abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);

        File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);
        AssetDatabase.Refresh();
    }

    public void UploadAllABFiles()
    {
        DirectoryInfo directory = Directory.CreateDirectory("E:\\VR\\AR\\AssetBundles\\"+ targetStrings[nowSelIndex] + "\\");
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


    public async void FtpUploadFiles(string filePath, string fileName)
    {
        await Task.Run(() => {
            try
            {
                FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/"+ targetStrings[nowSelIndex] + "/" + fileName)) as FtpWebRequest;
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
