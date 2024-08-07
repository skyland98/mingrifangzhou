using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaMgr : BaseManager<LuaMgr>
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

    public void Init()
    {
        if(luaEnv != null)
            return;
        luaEnv = new LuaEnv();
        luaEnv.AddLoader(MyCustomLoader);
        luaEnv.AddLoader(MyCustomABLoader);
    }

    public void DoLuaFile(string fileName)
    {
        string str = string.Format("require('{0}')",fileName);
        DoString(str);
    }


    public void DoString(string str)
    {
        if(luaEnv == null)
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

    private byte[] MyCustomLoader(ref string filePath)
    {
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

    private byte[] MyCustomABLoader(ref string filePath)
    {
        //Debug.Log("����AB�������ض���");
        //string path = Application.streamingAssetsPath + "/lua";
        //AssetBundle ab = AssetBundle.LoadFromFile(path);
        //TextAsset tx = ab.LoadAsset<TextAsset>(filePath +".lua");
        //return tx.bytes;

        TextAsset lua = ABMgr.Instance.LoadRes("lua", filePath + ".lua") as TextAsset;

        if (lua != null)
            return lua.bytes;
        else
            Debug.Log("MyCustomABLoader�ض���ʧ�ܣ��ļ���Ϊ" + filePath);

        return null;
    }
}
