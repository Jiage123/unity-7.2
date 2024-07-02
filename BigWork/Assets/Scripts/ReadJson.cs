using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadJson : MonoBehaviour
{
    public Toggle[] toggles = new Toggle[4];//选项
    public Text scoreText;//得分显示
    public Image selectPanel;//选择题显示
    public Text titleText;//选择题题目显示
    DataVO data;
    int index = 0;
    public Button nextbutton;
    List<string> answers = new List<string>();//存正确答案
    string select = "";//记录每次选的答案
    List<string>selctanswer = new List<string>();
    int score = 0;//统计分数
    void Start()
    {
        selectPanel.gameObject.SetActive(false);
       TextAsset text= Resources.Load<TextAsset>("JsonTest");
        string s=text.text;
        scoreText.gameObject.SetActive(false);
        //print(s);
        data= JsonUtility.FromJson<DataVO>(s);//序列化数据对象给其赋值
        for (int i = 0; i < data.Test.Count; i++)
        {
            answers.Add(data.Test[i].answer);
        }
    }
    //打开选择题界面
    public void OpenSelectImage() {
        index = 0;
        score = 0;
        select="";
        selctanswer.Clear();
        selectPanel.gameObject.SetActive(true);
        nextbutton.transform.GetChild(0).GetComponent<Text>().text = "下一题";
        titleText.text = data.Test[index].Title;//选择题第一题的题目
        // 选择题第一题的选项
        for (int i = 0; i < 4; i++)
        {
            toggles[i].transform.GetChild(1).GetComponent<Text>().text = data.Test[index].Select[i];
            toggles[i].isOn = false;
        }
    }
    //下一题
    public void NextClick() {
        if (string.IsNullOrEmpty(select))
        {
            return;
        }
        index++;
        selctanswer.Add(select);
        select = "";
        if (index==4)
        {
            nextbutton.transform.GetChild(0).GetComponent<Text>().text = "答题完毕";
        }
        if (index==5)
        {
            for (int i = 0; i < answers.Count; i++)
            {
                if (answers[i]==selctanswer[i])
                {
                    score += 10;
                }
            }
            scoreText.text = "得分:"+score;
            selectPanel.gameObject.SetActive(false);
            scoreText.gameObject.SetActive(true);
            return;
        }
        titleText.text = data.Test[index].Title;//选择题第index+1题的题目
        // 选择题第index+1题的选项
        for (int i = 0; i < 4; i++)
        {
            toggles[i].transform.GetChild(1).GetComponent<Text>().text = data.Test[index].Select[i];
            toggles[i].isOn = false;
        }
    }
    void Update()
    {
        
    }
    public void SelectA(bool b)
    {
        if (b)
        {
            select = "A";
            toggles[0].transform.GetChild(0).GetComponent<Image>().color = Color.green;
        }
        else
        {
            toggles[0].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }
    public void SelectB(bool b)
    {
        if (b)
        {
            select = "B";
            toggles[1].transform.GetChild(0).GetComponent<Image>().color = Color.green;
        }
        else
        {
            toggles[1].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }
    public void SelectC(bool b)
    {
        if (b)
        {
            select = "C";
            toggles[2].transform.GetChild(0).GetComponent<Image>().color = Color.green;
        }
        else
        {
            toggles[2].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }
    public void SelectD(bool b)
    {
        if (b)
        {
            select = "D";
            toggles[3].transform.GetChild(0).GetComponent<Image>().color = Color.green;
        }
        else
        {
            toggles[3].transform.GetChild(0).GetComponent<Image>().color = Color.white;
        }
    }
}
