using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Lesson_MD5 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(Application.persistentDataPath);
        ABUpdateMgr.Instance.CheckUpdate((isOver) =>
        {
            if(isOver)
            {
                print("�����½��������ؽ�����");
            }
            else 
            {
                print("���������ʾ��Ҽ������");
            }
        }, (str) =>
        {
            print(str);
        });

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private string GetMD5(string filePath)
    {
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            //�õ����ݵ�MD5�� 16���ֽ� ����
            byte[] md5Info = md5.ComputeHash(file);

            file.Close();

            //��16���ֽ�ת��Ϊ 16���� ƴ�ӳ��ַ��� Ϊ�˼���md5��ĳ���
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < md5Info.Length; i++)
            {
                sb.Append(md5Info[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }

}
