using System.Collections;
using System.Collections.Generic;
using Arknights.Tools;
using System.Reflection;
using System;

namespace Arknights.Event
{
    public class EventSystem : Single<EventSystem>
    {
        // 事件 - 对象 - 方法  <Event, <Listener, List<MethodInfo>>>
        private Dictionary<Type, Dictionary<Listener, List<MethodInfo>>> dic = new Dictionary<Type, Dictionary<Listener, List<MethodInfo>>>();

        /**
         * 添加监听器
         */
        public void AddListener(Listener listener)
        {
            MethodInfo[] methodInfos = listener.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (methodInfo.ReturnType == typeof(void) && Attribute.GetCustomAttribute(methodInfo, typeof(EventHandler)) != null)
                {
                    // 判断是否符合要求
                    if (methodInfo.GetParameters().Length != 1)
                    {
                        throw new Exception("方法: " + methodInfo.Name + " 的参数数量不为1!");
                    }
                    Type eventType = methodInfo.GetParameters()[0].ParameterType;
                    if (eventType.IsAbstract)
                    {
                        throw new Exception("方法: " + methodInfo.Name + " 未使用可实例化参数!");
                    }
                    if (!eventType.IsSubclassOf(typeof(Event)))
                    {
                        throw new Exception("方法: " + methodInfo.Name + " 的参数未继承于Event!");
                    }
                    // 符合条件的添加方式
                    if (!dic.TryGetValue(eventType, out Dictionary<Listener, List<MethodInfo>> listenerList))
                    {
                        listenerList = new Dictionary<Listener, List<MethodInfo>>();
                        dic.Add(eventType, listenerList);
                    }
                    if (!listenerList.TryGetValue(listener, out List<MethodInfo> methods))
                    {
                        methods = new List<MethodInfo>();
                        listenerList.Add(listener, methods);
                    }
                    methods.Add(methodInfo);
                }
            }
        }

        /**
         * 删除监听器
         */
        public void RemoveListener(Listener listener)
        {
            MethodInfo[] methodInfos = listener.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (methodInfo.ReturnType == typeof(void) && Attribute.GetCustomAttribute(methodInfo, typeof(EventHandler)) != null && methodInfo.GetParameters().Length == 1)
                {
                    Type eventType = methodInfo.GetParameters()[0].ParameterType;
                    if (!eventType.IsAbstract && eventType.IsSubclassOf(typeof(Event)) && dic.TryGetValue(eventType, out Dictionary<Listener, List<MethodInfo>> listenerList) && listenerList.ContainsKey(listener))
                    {
                        listenerList.Remove(listener);
                    }
                }
            }
        }


        /**
         * 通知事件
         */
        public void Call(Event callEvent)
        {
            if (dic.TryGetValue(callEvent.GetType(), out Dictionary<Listener, List<MethodInfo>> listenerList))
            {
                foreach (KeyValuePair<Listener, List<MethodInfo>> entry in listenerList)
                {
                    Listener listener = entry.Key;
                    foreach (MethodInfo methodInfo in entry.Value)
                    {
                        methodInfo.Invoke(listener, new object[] { callEvent });
                    }
                }
            }
        }
    }
}

