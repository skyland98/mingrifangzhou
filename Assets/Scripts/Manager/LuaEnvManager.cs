using Arknights.Tools;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

namespace Arknights.Manager
{
    /// <summary>
    /// Lua管理器
    /// 提供 lua解析器
    /// 保证解析器的唯一性
    /// </summary>
    public class LuaEnvManager : Single<LuaEnvManager>
    {
        private LuaEnv luaEnv;

        /// <summary>
        /// 得到Lua中的_G
        /// </summary>
        public LuaTable Global
        {
            get
            {
                return luaEnv.Global;
            }
        }

        // 初始化解析器
        public void Init()
        {
            if (luaEnv != null)
                return;
            luaEnv = new LuaEnv();

            //加载lua脚本 重定向
            luaEnv.AddLoader(MyCustomLoader);
            luaEnv.AddLoader(MyCustomABLoader);
        }

        // 传入lua文件名 执行lua脚本
        public void DoLuaFile(string fileName)
        {
            string str = string.Format("require('{0}')", fileName);
            DoString(str);
        }

        // 执行Lua语言
        public void DoString(string str)
        {
            if (luaEnv == null)
            {
                Debug.Log("解析器未初始化");
                return;
            }

            luaEnv.DoString(str);
        }

        //释放lua垃圾
        public void Tick()
        {
            if (luaEnv == null)
            {
                Debug.Log("解析器未初始化");
                return;
            }
            luaEnv.Tick();
        }

        //销毁解析器
        public void Dispose()
        {
            if (luaEnv == null)
            {
                Debug.Log("解析器未初始化");
                return;
            }
            luaEnv.Dispose();
            luaEnv = null;
        }

        //自动执行
        private byte[] MyCustomLoader(ref string filePath)
        {
            //通过函数中的逻辑 去加载 Lua文件 
            //传入的参数 是 require执行的lua脚本文件名
            //拼接一个Lua文件所在路径
            string path = Application.dataPath + "/Lua/" + filePath + ".lua";
            //Debug.Log(path);

            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            else
            {
                Debug.Log("重定向失败");
            }

            return null;
        }

        //Lua脚本会放在AB包 
        //最终我们会通过加载AB包再加载其中的Lua脚本资源 来执行它
        //重定向加载AB包中的LUa脚本
        private byte[] MyCustomABLoader(ref string filePath)
        {
            //Debug.Log("进入AB包加载重定向");
            //string path = Application.streamingAssetsPath + "/lua";
            //AssetBundle ab = AssetBundle.LoadFromFile(path);
            //TextAsset tx = ab.LoadAsset<TextAsset>(filePath +".lua");
            //return tx.bytes;

            //通过我们的AB包管理器 加载的lua脚本资源
            TextAsset lua = ABMgr.Instance.LoadRes("lua", filePath + ".lua") as TextAsset;

            if (lua != null)
                return lua.bytes;
            else
                Debug.Log("MyCustomABLoader重定向失败，文件名为" + filePath);

            return null;
        }
    }
}
