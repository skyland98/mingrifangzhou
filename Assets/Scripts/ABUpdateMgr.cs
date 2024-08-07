using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ABUpdateMgr : MonoBehaviour
{
    private static ABUpdateMgr instance;
    public static ABUpdateMgr Instance
    {  
        get 
        { 
            if(instance == null)
            {
                GameObject obj = new GameObject("ABUpdateMgr");
                instance= obj.AddComponent<ABUpdateMgr>();
            }
            return instance; 
        } 
    }

    //���ڴ洢Զ��AB����Ϣ���ֵ� �� ���ضԱȼ�����ɸ�������
    private Dictionary<string,ABInfo> remoteABInfo = new Dictionary<string,ABInfo>();

    //���ڴ洢����AB����Ϣ���ֵ� 
    private Dictionary<string, ABInfo> localABInfo = new Dictionary<string, ABInfo>();

    //����Ǵ�����AB���б��ļ� 
    private List<string> downLoadList = new List<string>();

    private string serverIP = "ftp://192.168.124.5";
    public void CheckUpdate(UnityAction<bool> overCallBack,UnityAction<string> updateInfoCallBack)
    {
        remoteABInfo.Clear();
        localABInfo.Clear();
        downLoadList.Clear();


        DownLoadABCompareFile((isOver) => {
            updateInfoCallBack("��ʼ������Դ");
            if (isOver)
            {
                updateInfoCallBack("�Ա��ļ����ؽ���");

                string remoteInfo = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
                updateInfoCallBack("����Զ�˶Ա��ļ�");
                GetRemoteABCompareFileInfo(remoteInfo, remoteABInfo);
                updateInfoCallBack("����Զ�˶Ա��ļ����");

                GetLocalABCompareFileInfo(() =>
                {
                    updateInfoCallBack("�������ضԱ��ļ����");

                    updateInfoCallBack("��ʼ�Ա�");
                    foreach (string abName in remoteABInfo.Keys)
                    {
                        if(!localABInfo.ContainsKey(abName))
                            downLoadList.Add(abName);
                        else
                        {
                            if (localABInfo[abName].md5 != remoteABInfo[abName].md5)
                                downLoadList.Add(abName);

                            localABInfo.Remove(abName);
                        }
                    }
                    updateInfoCallBack("�Ա����");
                    updateInfoCallBack("ɾ�����õ�AB��");
                    //ɾ�����õ�AB��
                    foreach (string abName in localABInfo.Keys)
                    {
                        if (File.Exists(Application.persistentDataPath + "/" + abName))
                            File.Delete(Application.persistentDataPath + "/" + abName);
                    }

                    updateInfoCallBack("���غ͸���AB��");
                    DownLoadABFile((isOver) =>
                    {
                        if(isOver)
                        {
                            updateInfoCallBack("���±���AB��Ϊ����");
                            File.WriteAllText(Application.persistentDataPath + "/ABCompareInfo.txt",remoteInfo);
                        }
                        overCallBack(isOver);
                    });

                });
            }
            else
            {
                overCallBack(false);
            }
        });
       
    }

    public async void DownLoadABCompareFile(UnityAction<bool> overCallBack)
    {
        print(Application.persistentDataPath);

        string localPath = Application.persistentDataPath + "/";
        bool isOver = false;
        int reDownLoadMaxNum = 5;
        while(!isOver &&  reDownLoadMaxNum > 0)
        {
            await Task.Run(() => {
                isOver = DownLoadFile("ABCompareInfo.txt", localPath + "/ABCompareInfo_TMP.txt");
            });
            --reDownLoadMaxNum;
        }

        overCallBack?.Invoke(isOver);    
        
    }

    //����Զ�˺ͱ��صĶԱ��ļ� �������ֵ���
    public void GetRemoteABCompareFileInfo(string info, Dictionary<string, ABInfo> ABInfo)
    {
        //string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");

        string[] strs = info.Split("|");

        string[] infos = null;
        for (int i = 0; i < strs.Length; i++)
        {
            infos = strs[i].Split(" ");
            ABInfo abInfo = new ABInfo(infos[0], infos[1], infos[2]);
            //��¼ÿһ��Զ�˵�AB����Ϣ �������Ա�
            ABInfo.Add(infos[0], abInfo);
        }

        print("AB���Ա��ļ� ���ؽ���");
    }

    public void GetLocalABCompareFileInfo(UnityAction overCallBack)
    {
        if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
        {
            StartCoroutine(GetLocalABCompareFileInfo(Application.persistentDataPath + "/ABCompareInfo.txt", overCallBack));
        }
        else if (File.Exists(Application.streamingAssetsPath + "/ABCompareInfo.txt"))
        {
            StartCoroutine(GetLocalABCompareFileInfo(Application.streamingAssetsPath + "/ABCompareInfo.txt", overCallBack));
        }
        else
            overCallBack();
    }

    private IEnumerator GetLocalABCompareFileInfo(string filePath, UnityAction overCallBack)
    {
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        yield return req.SendWebRequest();
        
        GetRemoteABCompareFileInfo(req.downloadHandler.text,localABInfo);

        overCallBack();
    }


    public async void DownLoadABFile(UnityAction<bool> overCallBack)
    {
        

        string localPath = Application.persistentDataPath + "/";
        //�Ƿ����سɹ�
        bool isOver;
        //���سɹ����б�
        List<string> tmpList = new List<string>();

        int reDownLoadMaxNum = 5;
        int downLoadOverNum = 0;
        while(downLoadList.Count > 0 && reDownLoadMaxNum >0)
        {
            for (int i = 0; i < downLoadList.Count; i++)
            {
                isOver = false;
                await Task.Run(() => {
                    isOver = DownLoadFile(downLoadList[i], localPath + downLoadList[i]);
                });

                if (isOver)
                {
                    print("���ؽ���" + ++downLoadOverNum + "/" + downLoadList.Count);
                    tmpList.Add(downLoadList[i]);//���سɹ���¼����
                }
            }

            for (int i = 0; i < tmpList.Count; i++)
                downLoadList.Remove(tmpList[i]);

            --reDownLoadMaxNum;
        }

        //�����ⲿ�������     
        overCallBack(downLoadList.Count == 0);

        
    }



    private bool DownLoadFile(string fileName,string localPath)
    {
        try
        {
            string pInfo =
#if UNITY_IOS
"IOS";
#elif UNITY_ANDROID
"Android";
#else
"PC";
#endif

            FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + "/AB/"+pInfo+"/" + fileName)) as FtpWebRequest;
            NetworkCredential n = new NetworkCredential("Liu", "Liu123");
            req.Credentials = n;

            req.Proxy = null;
            req.KeepAlive = false;
            req.Method = WebRequestMethods.Ftp.DownloadFile;
            req.UseBinary = true;

            FtpWebResponse res = req.GetResponse() as FtpWebResponse;
            Stream downLoadStream = res.GetResponseStream();

            using (FileStream file = File.Create(localPath))
            {
                //һ��һ������
                byte[] bytes = new byte[2048];

                int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                while (contentLength != 0)
                {
                    file.Write(bytes, 0, contentLength);
                    contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                }

                file.Close();
                downLoadStream.Close();

                print(fileName + "���سɹ�" );
                return true;
            }
        }
        catch (Exception e)
        {
            print(fileName +"����ʧ��" + e.Message);
            return false;
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public class ABInfo
    {
        public string name;
        public long size;
        public string md5;

        public ABInfo(string name,string size,string md5)
        {
            this.name = name;
            this.size = long.Parse(size);
            this.md5 = md5;
        }
    }
}
