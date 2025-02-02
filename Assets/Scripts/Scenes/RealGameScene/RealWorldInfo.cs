using System.Collections.Generic;

public abstract class RealWorldInfo 
{
    public List<string> dialog;
    public List<int> score;
}

public class FirstWorldInfo : RealWorldInfo
{
    public FirstWorldInfo()
    {
        dialog = new List<string>()
        {
            "����ϱ� ¦�� �����鼭 ��� �������� �ϴ� �ž�?",
            "�� ��¥ ���簨�̶��� 1�� ����.",
            "�ܸ� �ɷµ� Ư���� �� ���� ������� �� ����ϰھ�.",
            "���� ������ �����̳� ���� �� �˾�?",
            "�� ���ʿ� ������ ����̾�.",
            "�ʴ� ��ü �� ���ϴ�?",
        };

        score = new List<int>()
        {
            -20, -20, -20, -20, -30, -40
        };
    }
}

public class SecondWorldInfo : RealWorldInfo
{
    public SecondWorldInfo()
    {
        dialog = new List<string>()
        {
            "�� ��¥ ��� �ǰ��ϰ� �����.",
            "�� �� û���� ���ϴ�?",
            "��¥ �ʶ� ��ȭ�� �ϳ��� �� �ȴ�.",
            "�� �� �ڱ����常 ��?",
            "�� ���� ����� ���� ������ �ϰھ�?",
            "�� �ֱ� ȥ�� �����ž�."
        };

        score = new List<int>()
        {
            -20, -20, -20, -30, -40, -40
        };
    }
}

public class ThirdWorldInfo : RealWorldInfo
{
    public ThirdWorldInfo()
    {
        dialog = new List<string>()
        {
            "�� �������δ� �� ���� �� ���� �ʾ�?",
            "��¥ �ƴ� �� ����?",
            "�ʳ� �������� ������ ��� ���� �ϼ��� �ٵ���",
            "�� �����߾�?",
            "�̰͵� �� ��?",
            "�� �� �Ƕ�� �ϴ� �Ҹ���.",
            "�ð� ����� �װ�",
            "������ ����ó�� ��ž�?",
            "������ �ʹ� ŭ ��뿴����",
            "�������� ���鷡?"
        };

        score = new List<int>()
        {
            -10, -10, -20, -20, -20, -20, -20, -20, -30, -30
        };
    }
}
