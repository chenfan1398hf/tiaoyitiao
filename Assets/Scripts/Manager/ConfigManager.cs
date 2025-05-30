using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using System.Linq;
using System.IO;

public class ConfigManager
{

    public List<QuestionCfg> TaskInfoCfg = new List<QuestionCfg>();                             //任务配置
    //读取游戏配置表
    public void InitGameCfg()  //初始化自定义的游戏配置表
    {
        //故意未Try 这部分加载 如果配置表异常会直接关闭程序
        string txt = FileTool.Read_Txt("question");
        TaskInfoCfg = JsonMapper.ToObject<List<QuestionCfg>>(txt);

        return;
    }

    //public LanguageCfg GetLanguageCfgByKey(string key)
    //{
    //    return mLanguageCfg.Find((item) => item.key == key);
    //}
    public QuestionCfg GetLanguageCfgByKey(int key)
    {
        return TaskInfoCfg.Find((item) => item.ID == key);
    }
}