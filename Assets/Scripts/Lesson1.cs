using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

/// <summary>
/// Lua������
/// �ṩlua������
/// ��֤��������Ψһ��
/// </summary>

//�޲��޷��ص�ί��
public delegate void CustomCall();

public class Lesson1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        LuaMgr.GetInstance().Init();
        LuaMgr.GetInstance().DoLuaFile("Main");

        int i = LuaMgr.GetInstance().Global.Get<int>("testNumber");
        Debug.Log( "testNumber:"+i);

        CustomCall call = LuaMgr.GetInstance().Global.Get<CustomCall>("testFun");
        call();
        //Lua������ ��Unity��ִ��Lua
        //LuaEnv luaEnv = new LuaEnv();
        //luaEnv.DoString("print('�������')");

        //luaEnv.AddLoader(MyCustomLoader);
        //luaEnv.DoString("require('Main')");
        //���Lua������û���ֶ��ͷŵĶ��� ��������
        //luaEnv.Tick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private byte[] MyCustomLoader(ref string filePath)
    //{
    //    string path = Application.dataPath + "/Lua/" + filePath + ".lua";
    //    Debug.Log(path);

    //    if(File.Exists(path))
    //    {
    //        return File.ReadAllBytes(path);
    //    }
    //    else
    //    {
    //        Debug.Log("�ض���ʧ��");
    //    }

    //    return null;
    //}

}
