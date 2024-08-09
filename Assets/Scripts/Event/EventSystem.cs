using System.Collections;
using System.Collections.Generic;
using Arknights.Tools;
using System.Reflection;
using System;

namespace Arknights.Event
{
    public class EventSystem : Single<EventSystem>
    {
        // �¼� - ���� - ����  <Event, <Listener, List<MethodInfo>>>
        private Dictionary<Type, Dictionary<Listener, List<MethodInfo>>> dic = new Dictionary<Type, Dictionary<Listener, List<MethodInfo>>>();

        /**
         * ��Ӽ�����
         */
        public void AddListener(Listener listener)
        {
            MethodInfo[] methodInfos = listener.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn);
            foreach (MethodInfo methodInfo in methodInfos)
            {
                if (methodInfo.ReturnType == typeof(void) && Attribute.GetCustomAttribute(methodInfo, typeof(EventHandler)) != null)
                {
                    // �ж��Ƿ����Ҫ��
                    if (methodInfo.GetParameters().Length != 1)
                    {
                        throw new Exception("����: " + methodInfo.Name + " �Ĳ���������Ϊ1!");
                    }
                    Type eventType = methodInfo.GetParameters()[0].ParameterType;
                    if (eventType.IsAbstract)
                    {
                        throw new Exception("����: " + methodInfo.Name + " δʹ�ÿ�ʵ��������!");
                    }
                    if (!eventType.IsSubclassOf(typeof(Event)))
                    {
                        throw new Exception("����: " + methodInfo.Name + " �Ĳ���δ�̳���Event!");
                    }
                    // ������������ӷ�ʽ
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
         * ɾ��������
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
         * ֪ͨ�¼�
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

