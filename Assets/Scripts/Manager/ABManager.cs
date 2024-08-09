using Arknights.Tools;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Arknights.Manager
{
    public class ABManager : Single<ABManager>
    {
        /// <summary>
        /// ���ع�����δж�ص�AB��
        /// </summary>
        private Dictionary<string, AssetBundle> loadedDic = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// ��һ���ܰ�
        /// </summary>
        private AssetBundle single;

        /// <summary>
        /// �ܵĹ����嵥
        /// </summary>
        private AssetBundleManifest manifest;

        /// <summary>
        /// ab����·��
        /// </summary>
        private string abPath = "";

        public string ABPath
        {
            get
            {
                if (abPath == "")
                {
#if UNITY_STANDALONE_WIN
                    abPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID || UNITY_IOS
                    abPath = Application.persistentDataPath + "/";
#endif
                }
                return abPath;
            }
        }

        /// <summary>
        /// �ܵ�AB��������
        /// </summary>
        public string SingleABName
        {
            get
            {
                //����ƽ̨��ͬ���ܵ�AB�������ֿ��ܲ�ͬ
#if UNITY_STANDALONE_WIN
                return "PC";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "IOS";
#else
            Debug.LogError("δָ����ƽ̨������");
#endif
                return "";
            }
        }

        /// <summary>
        /// ����AB��
        /// </summary>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abName)
        {
            //Ҫ���������Ҫ�Ȼ�ȡ������Ҫ��ȡ�������ȼ��ص��ܵĹ����嵥

            //���ж��ܰ��Ƿ���ع�
            if (single == null)
            {
                if (!File.Exists(ABPath + SingleABName))
                {
                    Debug.LogWarning($"δ�ҵ�·��: {ABPath}{SingleABName}");
                    return null;
                }
                single = AssetBundle.LoadFromFile(ABPath + SingleABName);
            }

            //���жϹ����嵥�Ƿ���ع�
            if (manifest == null)
            {
                //���ܰ�����ع����嵥
                manifest = single.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            //��ȡҪ����AB����������
            string[] deps = manifest.GetAllDependencies(abName);
            //�������ݼ���������
            for (int i = 0; i < deps.Length; i++)
            {
                string depABName = deps[i];
                //����������Ǽ���AB���ģ� �ڵ��ñ�����Ǽ�����һ��AB��
                //����AB���໥����������£��ݹ�������ѭ��
                //LoadAssetBundle(depABName);

                //�ж��Ƿ���ع�
                if (!loadedDic.ContainsKey(depABName))
                {
                    AssetBundle depAB = AssetBundle.LoadFromFile(ABPath + depABName);
                    loadedDic.Add(depABName, depAB);
                }
            }

            //�Ƿ���ع�
            if (!loadedDic.TryGetValue(abName, out AssetBundle ab))
            {
                //û�ҵ��� δ���ع�������ع�֮��ж����, 
                //��Ҫ���¼���
                //���ظ�ab��ǰ����Ҫ�ȼ�������������
                if (File.Exists(ABPath + abName))
                {
                    //���������������
                    //�ڼ��ص�ǰ��AB��
                    ab = AssetBundle.LoadFromFile(ABPath + abName);
                    //Debug.Log("������ " + abName + " ��");
                    //�����ؽ�����AB����ӵ��ֵ���
                    loadedDic.Add(abName, ab);
                }
            }
            return ab;
        }


        /// <summary>
        /// ж��AB��
        /// </summary>
        public void UnloadAssetBundle(string abName, bool unloadAllObjects = false)
        {
            //�Ƿ������
            AssetBundle ab = null;
            if (loadedDic.TryGetValue(abName, out ab))
            {
                //ж��
                ab.Unload(unloadAllObjects);
                //������ֵ����Ƴ�
                loadedDic.Remove(abName);
                // Debug.Log("ж���ˣ�" + abName);
            }
        }

        /// <summary>
        /// ж��ȫ����AB��
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        public void UnloadAll(bool unloadAllObjects = false)
        {
            //�����ֵ��ֵ
            foreach (AssetBundle assetbundle in loadedDic.Values)
            {
                assetbundle.Unload(unloadAllObjects);
            }
            //����ֵ䣬����յĵ�����£���ֵ�Ի����ڣ�ֻ������ֵΪnull
            loadedDic.Clear();
        }


        /// <summary>
        /// ��ָ����AB���м���ָ�����ֵ���Դ TΪ��Դ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName, string abName)
            where T : Object
        {
            //�Ȼ�ȡAB��, ���ü���AB���ķ�����ȡAB������ʹ֮ǰ���ع��������ڲ��ж��˲����ظ��ļ���
            AssetBundle ab = LoadAssetBundle(abName);
            if (ab != null && ab.Contains(assetName))
            {
                //��AB���л�ȡ��Դ
                T asset = ab.LoadAsset<T>(assetName);
                return asset;
            }
            //���÷Ƿ��ͷ���
            //Object asset = LoadAsset(assetName, abName, typeof(T));
            //return asset as T;
            return default(T);
        }


        public Object LoadAsset(string assetName, string abName, System.Type type)
        {
            //�Ȼ�ȡAB��
            AssetBundle ab = LoadAssetBundle(abName);
            if (ab != null)
            {
                //��ab���л�ȡ��Դ
                Object asset = ab.LoadAsset(assetName, type);
                return asset;
            }
            return null;
        }
    }
}