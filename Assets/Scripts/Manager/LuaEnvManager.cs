using Arknights.Tools;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

namespace Arknights.Manager
{
    /// <summary>
    /// Lua������
    /// �ṩ lua������
    /// ��֤��������Ψһ��
    /// </summary>
    public class LuaEnvManager : Single<LuaEnvManager>
    {
        private LuaEnv luaEnv;

        /// <summary>
        /// �õ�Lua�е�_G
        /// </summary>
        public LuaTable Global
        {
            get
            {
                return luaEnv.Global;
            }
        }

        // ��ʼ��������
        public void Init()
        {
            if (luaEnv != null)
                return;
            luaEnv = new LuaEnv();

            //����lua�ű� �ض���
            luaEnv.AddLoader(MyCustomLoader);
            luaEnv.AddLoader(MyCustomABLoader);
        }

        // ����lua�ļ��� ִ��lua�ű�
        public void DoLuaFile(string fileName)
        {
            string str = string.Format("require('{0}')", fileName);
            DoString(str);
        }

        // ִ��Lua����
        public void DoString(string str)
        {
            if (luaEnv == null)
            {
                Debug.Log("������δ��ʼ��");
                return;
            }

            luaEnv.DoString(str);
        }

        //�ͷ�lua����
        public void Tick()
        {
            if (luaEnv == null)
            {
                Debug.Log("������δ��ʼ��");
                return;
            }
            luaEnv.Tick();
        }

        //���ٽ�����
        public void Dispose()
        {
            if (luaEnv == null)
            {
                Debug.Log("������δ��ʼ��");
                return;
            }
            luaEnv.Dispose();
            luaEnv = null;
        }

        //�Զ�ִ��
        private byte[] MyCustomLoader(ref string filePath)
        {
            //ͨ�������е��߼� ȥ���� Lua�ļ� 
            //����Ĳ��� �� requireִ�е�lua�ű��ļ���
            //ƴ��һ��Lua�ļ�����·��
            string path = Application.dataPath + "/Lua/" + filePath + ".lua";
            //Debug.Log(path);

            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                Debug.Log("�ض���ʧ��");
            }

            return null;
        }

        //Lua�ű������AB�� 
        //�������ǻ�ͨ������AB���ټ������е�Lua�ű���Դ ��ִ����
        //�ض������AB���е�LUa�ű�
        private byte[] MyCustomABLoader(ref string filePath)
        {
            //Debug.Log("����AB�������ض���");
            //string path = Application.streamingAssetsPath + "/lua";
            //AssetBundle ab = AssetBundle.LoadFromFile(path);
            //TextAsset tx = ab.LoadAsset<TextAsset>(filePath +".lua");
            //return tx.bytes;

            //ͨ�����ǵ�AB�������� ���ص�lua�ű���Դ
            TextAsset lua = ABMgr.Instance.LoadRes("lua", filePath + ".lua") as TextAsset;

            if (lua != null)
                return lua.bytes;
            else
                Debug.Log("MyCustomABLoader�ض���ʧ�ܣ��ļ���Ϊ" + filePath);

            return null;
        }
    }
}
