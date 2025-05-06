using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoSingleton<GameManager>
{
    #region 构造函数及其变量
    public GameManager()
    {
        configMag = new ConfigManager();
    }
    public static bool isDbugLog = true;
    public PlayerData playerData = null;                            //玩家数据（本地持久化）
    public ConfigManager configMag;
    private System.Random Random;                                   //随机种子
    private int TimeNumber = 0;
    private List<UnityAction> unityActionList = new List<UnityAction>();
    public bool isBattle = true;


    public static int TI_LI_MAX_NUMBER = 100;
    public static int TI_LI_CD_NUMBER = 600;

    #endregion

    private void Update()
    {
        foreach (var item in unityActionList)
        {
            item.Invoke();
        }
    }
    #region Awake()
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;//设置帧率为60帧
        GetLocalPlayerData();
        Random = new System.Random(Guid.NewGuid().GetHashCode());
    }
    #endregion



    private void Start()
    {
        this.InvokeRepeating("CheckTime", 0, 0.1f);
        BeginGame();
    }

    void CheckTime()
    {
        TimeNumber++;

        if (TimeNumber % 10 == 0)
        {
            UpdateTime();
        }
        if (TimeNumber % 20 == 0)
        {

        }


    }


    #region OnApplicationPause(bool pause)切屏感知
    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            if (isDbugLog)
                Debug.Log("切屏感知");
            SaveGame();
        }
    }
    #endregion

    #region OnApplicationQuit() 退出游戏感知
    public void OnApplicationQuit()
    {
        if (isDbugLog)
            Debug.Log("退出感知");
        SaveGame();

    }
    #endregion

    #region 获取本地数据
    public void GetLocalPlayerData()
    {
        playerData = PlayerData.GetLocalData();//读取本地持久化玩家数据(包括本土化设置)
        configMag.InitGameCfg();//读取配置表
        playerData.InitData();//根据配置表和本地数据初始化游戏数据
    }
    #endregion

    #region SaveGame() 保存玩家数据
    public void SaveGame()
    {
        //if(SocketManager.instance.socket!=null)
        //SocketManager.instance.socket.Disconnect();
        playerData.Save();
    }
    #endregion

    #region OnDestroy()
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    #endregion

    /// <summary>
    /// 注册一个update在这里跑
    /// </summary>
    /// <param name="_action"></param>
    public void AddUpdateListener(UnityAction _action)
    {
        unityActionList.Add(_action);
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImage(string id, Image image)
    {
        string path = "Icon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片--装备图标
    /// </summary>
    public void SpritPropEquipIcon(string id, Image image)
    {
        string path = "EquipIcon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }


    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, Image image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, SpriteRenderer image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 添加预制体
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fatherTransform"></param>
    /// <returns></returns>
    public GameObject AddPrefab(string name, Transform fatherTransform)
    {
        string newpath = "Prefab/" + name;
        GameObject obj = ObjPool.instance.GetObj(newpath, fatherTransform);
        obj.AddComponent<DesObj>();
        obj.GetComponent<DesObj>().InitDes(newpath);
        return obj;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(string name, GameObject gameObject)
    {
        string[] list = name.Split(new char[] { '(' });
        if (list.Length != 2)
        {
            string newpath = "Prefab/" + name;
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        else
        {
            string newpath = "Prefab/" + list[0];
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject prefabObj, GameObject gameObject, string _path = null)
    {
        ObjPool.instance.Recycle(prefabObj, gameObject, "Prefab/" + _path);
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject gameObject)
    {
        string name = gameObject.GetComponent<DesObj>().name;
        ObjPool.instance.Recycle(name, gameObject);
        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(SkeletonGraphic _skeletonGraphic, bool isLoop, string _spineName, bool isRest)
    {
        if (isRest)
        {
            _skeletonGraphic.AnimationState.ClearTracks();
            _skeletonGraphic.AnimationState.Update(0);
        }
        _skeletonGraphic.AnimationState.SetAnimation(0, _spineName, isLoop);

        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(Animator _animator, string _spineName, bool isRest)
    {
        //_animator.Play(_spineName, 0 ,0f);
        if (isRest)
        {
            //_animator.Update(0);
            _animator.Play(_spineName, 0, 0f);
        }
        else
        {
            _animator.Play(_spineName);
        }
        return;
    }
    /// <summary>
    /// 获取对象池内对象数据
    /// </summary>
    /// <returns></returns>
    public ObjPool.PoolItem GetPoolItem(string name)
    {
        string newpath = "Prefab/" + name;
        return ObjPool.instance.GetPoolItem(newpath); ;
    }
    /// <summary>
    /// 网络拉取图片
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_image"></param>
    /// <returns></returns>
    public IEnumerator GetHead(string _url, Image _image)
    {
        if (_url == string.Empty || _url == "")
        {
            _url = "https://p11.douyinpic.com/aweme/100x100/aweme-avatar/mosaic-legacy_3797_2889309425.jpeg?from=3067671334";
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
                _image.sprite = sprite;
                //Renderer renderer = plane.GetComponent<Renderer>();
                //renderer.material.mainTexture = texture;
            }
        }
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    public void CleraPlayerData()
    {
        PlayerData.ClearLocalData();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Editor/Tools/Clear")]
    static void CleraPlayerData1()
    {
        PlayerData.ClearLocalData();
    }
#endif
    private GameObject[] GetDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }

    public List<GameObject> boxList = new List<GameObject>();
    public GameObject playerObj;
    public Jiantou jiantou;
    public GameObject datiPanel;
    public GameObject namePanel;
    public GameObject gamePanel;
    private bool isJump = false;
    public Vector3 shanggeVec = new Vector3(-7.12f, -1.9f, 0f);
    //生成箱子
    public void AddBox()
    {
        datiPanel.SetActive(false);
        endPanel.SetActive(false);
        boxList.Clear();
        float oldX = -7.12f;
        for (int i = 0; i < configMag.TaskInfoCfg.Count; i++)
        {
            var obj = AddPrefab("Box", GameObject.Find("Boxs").transform);
            obj.transform.Find("xx").GetComponent<Box>().InitBox(i + 1);
            obj.transform.localPosition = Vector3.zero;
            float randx = Util.randomFloat(4.5f, 5f);
            float randy = Util.randomFloat(-1f, 1f);
            oldX += randx;
            obj.transform.localPosition += new Vector3(oldX, randy, 0);
            boxList.Add(obj);
            if (i < playerData.timuIndex)
            {
                obj.transform.Find("xx").gameObject.SetActive(false);
            }
        }
    }
    public void BeginGame()
    {
        foreach (var item in boxList)
        {
            Destroy(item);
        }
        AddBox();
        //playerObj.transform.localPosition = new Vector3(-7.12f, 0f, 0f);
        //boxIndex = playerData.timuIndex;
        maxNumber = 0;
        CreatNamePanel();
        if (playerData.timuIndex == 0)
        {
            playerObj.transform.localPosition = new Vector3(-7.12f, 0f, 0f);
        }
        else
        {
            playerObj.transform.position = boxList[playerData.timuIndex - 1].transform.Find("Pront").position;
        }
       
    }
    public void JumpPlayer()
    {
        if (isJump)
        {
            return;
        }
        isJump = true;
        int state = jiantou.GetState();
        shanggeVec = playerObj.transform.position;
        if (state == 3)
        {
            playerObj.transform.DOJump(boxList[playerData.timuIndex].transform.Find("Pront").position, 3, 1, 1f).SetEase(Ease.Linear);
            playerData.timuIndex++;
        }
        else if (state == 1)
        {
            Vector3 vector3 = boxList[playerData.timuIndex].transform.Find("Pront").position;
            vector3.x += 2f;
            playerObj.transform.DOJump(vector3, 3, 1, 1f).SetEase(Ease.Linear);
            playerData.timuIndex++;
        }
        else if (state == 2)
        {
            Vector3 vector3 = boxList[playerData.timuIndex].transform.Find("Pront").position;
            vector3.x -= 2f;
            playerObj.transform.DOJump(vector3, 3, 1, 1f).SetEase(Ease.Linear);
            playerData.timuIndex++;
        }
        this.Invoke("EndJump", 3f);
    }
    private void EndJump()
    {
        isJump = false;
    }
    //打开答题界面
    private int duiDeNumber = 0;
    private GameObject xxObj;
    private GameObject xxtexiaoObj;
    private int maxNumber = 0;
    public GameObject endPanel;
    public void OpenDatiPanel(int id, GameObject obj, GameObject texiaoObj)
    {
        datiPanel.SetActive(true);
        var cfg = configMag.GetLanguageCfgByKey(id);
        datiPanel.transform.Find("Image/Text (Legacy)").GetComponent<Text>().text = cfg.msg;
        datiPanel.transform.Find("Image/Button1/Text (Legacy)").GetComponent<Text>().text = cfg.Aa;
        datiPanel.transform.Find("Image/Button2/Text (Legacy)").GetComponent<Text>().text = cfg.Ab;
        datiPanel.transform.Find("Image/Button3/Text (Legacy)").GetComponent<Text>().text = cfg.Ac;
        datiPanel.transform.Find("Image/Button4/Text (Legacy)").GetComponent<Text>().text = cfg.Ad;
        duiDeNumber = cfg.right;
        xxObj = obj;
        xxtexiaoObj = texiaoObj;
    }
    public void ButtonClick(int _index)
    {
        if (_index == duiDeNumber)
        {
            datiPanel.SetActive(false);
            xxObj.SetActive(false);
            xxtexiaoObj.SetActive(true);
            maxNumber++;
            if (playerData.timuIndex >= configMag.TaskInfoCfg.Count)
            {
                endPanel.SetActive(true);
            }
        }
        else
        {
            //BeginGame();
            GetShangCiVec();
        }
    }
    public void EndGame()
    {
        // 在编辑器中停止播放模式
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 在构建的应用中退出
        Application.Quit();
#endif
    }
    public void GetShangCiVec()
    {
        playerObj.transform.DOJump(shanggeVec, 3, 1, 1f).SetEase(Ease.Linear);
        playerData.timuIndex--;
        datiPanel.SetActive(false);
    }
    //输入名字界面
    public void CreatNamePanel()
    {
        if (playerData.playerName == string.Empty)
        {
            namePanel.SetActive(true);
        }
    }
    public void GetName(string _name)
    {
        playerData.playerName = _name;
    }
    public void QueRenName()
    {
        namePanel.SetActive(false);
    }
    //刷新倒计时
    public void UpdateTime()
    {
        playerData.countDownTime--;
        gamePanel.transform.Find("Text (Legacy)").GetComponent<Text>().text = "倒计时：" + DateTimeUtil.secondToHHMMSS(playerData.countDownTime);
        gamePanel.transform.Find("Text (Legacy) (1)").GetComponent<Text>().text = "姓名：" + playerData.playerName;

        endPanel.transform.Find("Image/Text (Legacy) (1)").GetComponent<Text>().text = "倒计时：" + DateTimeUtil.secondToHHMMSS(playerData.countDownTime);
        endPanel.transform.Find("Image/Text (Legacy) (2)").GetComponent<Text>().text = "姓名：" + playerData.playerName;
    }
}
