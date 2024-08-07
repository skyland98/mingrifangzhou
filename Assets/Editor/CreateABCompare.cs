using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;



public class CreateABCompare 
{
    //[MenuItem("AB������/�����Ա��ļ�")]
    public static void CreateABCompareFile()
    {
        DirectoryInfo directory = Directory.CreateDirectory("E:\\VR\\AR\\AssetBundles\\PC\\");
        FileInfo[] fileInfos = directory.GetFiles();

        //���ڴ洢��Ϣ�� �ַ���
        string abCompareInfo = "";

        foreach (FileInfo info in fileInfos)
        {
            //û�к�׺�Ĳ���AB��
            if(info.Extension == "")
            {
                //Debug.Log(info.Name);
                abCompareInfo += info.Name + " " + info.Length + " " + GetMD5(info.FullName);
                abCompareInfo += "|";
            }
        }

        abCompareInfo = abCompareInfo.Substring(0,abCompareInfo.Length - 1);

        //Debug.Log(abCompareInfo);
        File.WriteAllText("E:\\VR\\AR\\AssetBundles\\PC\\ABCompareInfo.txt", abCompareInfo);
        Debug.Log("AB���Ա��ļ����ɳɹ�");
    }

    public static string GetMD5(string filePath)
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

}
