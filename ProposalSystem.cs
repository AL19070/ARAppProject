using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ProposalSystem : MonoBehaviour
{
    /// <summary>
    /// マーカー用オブジェクトのプレハブ
    /// </summary>
    [SerializeField] public GameObject[] _arPrefabs;

    /// <summary>
    /// ARTrackedImageManager
    /// </summary>
    [SerializeField] public ARTrackedImageManager _imageManager;

    /// <summary>
    /// マーカー用オブジェクトのプレハブと文字列を紐づけた辞書
    /// </summary>
    /// readonly
    public Dictionary<string, GameObject> _markerNameAndPrefabDictionary = new Dictionary<string, GameObject>();

    public static int textState = 1;

    public static int bottonCount = 1;

    public ARSessionOrigin aRSessionOrigin;

    Quaternion markerFrontRotationTmp;

    //Dropdownを格納する変数 強調表示のドロップダウン
    [SerializeField] private TMP_Dropdown dropdown;
    
    //商品属性のドロップダウン1
    [SerializeField] private TMP_Dropdown attributionDropdown1;
    //商品属性のドロップダウン1
    [SerializeField] private TMP_Dropdown attributionDropdown2;
    //商品属性のドロップダウン1
    [SerializeField] private TMP_Dropdown attributionDropdown3;

    [SerializeField] private Button pageChangeButton;

    //　アイテムデータベース
    [SerializeField]
    private ItemDataBase itemDataBase;

    //ターゲットリスト処理で使うリスト
    public List<Item> targetList;

    //該当する商品の名前だけのリスト
    private List<string> correspondItemList;

    //表示する商品の属性
    public List<int> textAttributionList = new List<int>() {1,2,3,4,5,6};

    private string readItemName;
    private GameObject readItemObject;

    psSQL ps;

    public int kanrenState; 

    public void Start()
    {
        kanrenState = 1;
        readItemName = "";
        markerFrontRotationTmp = new Quaternion(0f, 0f, 0f, 0f);
        //該当する商品の初期化
        targetList = new List<Item>();
        
        ps = new psSQL();
        ps.FirstConnect();
        
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        //辞書を作る 画像の名前とARオブジェクトのPrefabを紐づける
        for (var i = 0; i < _arPrefabs.Length; i++)
        {
            var arPrefab = Instantiate(_arPrefabs[i]);
            _markerNameAndPrefabDictionary.Add(_imageManager.referenceLibrary[i].name, arPrefab);
            arPrefab.SetActive(false);
        }
        
        //プルダウンメニューのテキストセット
        for (int i = 0; i < textAttributionList.Count; i++)
        {
            dropdown.options[i + 1].text = GetDropdownText(textAttributionList[i]);
        }
        
        if (Menuselect.scene == 4)
        {
            correspondItemList = new List<string>() { "セルベール整胃錠 12錠", "第一三共胃腸薬プラス 錠剤 50錠" };
        }
        else if (Menuselect.scene == 5)
        {
            correspondItemList = new List<string>() { "スクラート胃痛薬 36錠", "ブスコパンA錠 20錠","サクロンQ 6錠" };
        }
        else if (Menuselect.scene == 6)
        {
            correspondItemList = new List<string>() { "コーラック 120錠", "アクアナチュラル便秘薬 132錠", "3Aマグネシア 90錠","スルーラックマグネシウム 30錠" };
        }
        else if(Menuselect.scene == 7)
        {
            correspondItemList = new List<string>() { "VWX", "ABC" };
        }
        else if (Menuselect.scene == 12)
        {
            correspondItemList = new List<string>() { "バイエルアスピリン 30錠", "新セデス錠 40錠" };
        }
        else if (Menuselect.scene == 13)
        {
            correspondItemList = new List<string>() { "イブA錠 90錠", "ルキノン解熱鎮痛薬 60錠", "クイック頭痛薬DX 20錠" };
        }
        else if (Menuselect.scene == 14)
        {
            correspondItemList = new List<string>() { "ノーシンアセトアミノフェン 48錠", "タイレノールA 20錠", "リングルN 20錠", "ナロン錠 48錠" };
        }
        pageChangeButton.GetComponentInChildren<TextMeshProUGUI>().text = Menuselect.scene.ToString();
        
    }

    private void OnDisable()
    {
        _imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //ターゲットの初期化
    public void TargetReset()
    {
        readItemName = "";
        //readItemObject
        foreach (string correspondItem in correspondItemList)
        {
            var arObject = _markerNameAndPrefabDictionary[correspondItem];
            arObject.SetActive(false);
        }
    }

    /// <summary>
    /// 認識した画像マーカーに応じて紐づいたARオブジェクトを表示
    /// </summary>
    /// <param name="trackedImage">認識した画像マーカー</param>
    private void ActivateARObject(ARTrackedImage trackedImage)
    {
        //認識した画像マーカーの名前を使って辞書から任意のオブジェクトを引っ張り出す
        var arObject = _markerNameAndPrefabDictionary[trackedImage.referenceImage.name];
        var imageMarkerTransform = trackedImage.transform;

        //位置合わせ
        if (Menuselect.scene < 3 && Menuselect.scene != 1)
        {
            Vector3 pos = imageMarkerTransform.transform.position;
            pos.y += 0.04f;
            imageMarkerTransform.transform.position = pos;
        }

        //ARオブジェクトが揺れる原因ここかも？
        //画像を認識したとき
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            
            //認識したものが該当する商品かどうかの判定
            if (correspondItemList.Contains(trackedImage.referenceImage.name))
            {
                //認識した画像の位置をビューポート座標に変換（画面左したを(0,0)、右上を(1,1)として）
                var targetScreenPos = aRSessionOrigin.camera.WorldToViewportPoint(imageMarkerTransform.transform.position);
                if (targetScreenPos.x > 0.33 && targetScreenPos.x < 0.67 && targetScreenPos.y > 0.40 && targetScreenPos.y < 0.60)
                {
                    var markerFrontRotation = imageMarkerTransform.rotation * Quaternion.Euler(90f, 0f, 0f);
                    if (readItemName == "")
                    {
                        targetList.Clear();

                        if (Menuselect.scene == 7)
                        {
                            targetList.AddRange(ps.SGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 4)
                        {
                            targetList.AddRange(ps.HGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 5)
                        {
                            targetList.AddRange(ps.ThreeGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 6)
                        {
                            targetList.AddRange(ps.FourGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 12)
                        {
                            targetList.AddRange(ps.ATwoGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 13)
                        {
                            targetList.AddRange(ps.AThreeGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 14)
                        {
                            targetList.AddRange(ps.AFourGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        

                        //認識した場所にオブジェクトをセット
                        markerFrontRotation.x = 0;
                        markerFrontRotation.z = 0;
                        markerFrontRotation.y = 0;
                        arObject.transform.SetPositionAndRotation(imageMarkerTransform.transform.position, markerFrontRotation);
                        arObject.SetActive(true);

                        markerFrontRotationTmp = markerFrontRotation;
                        readItemName = trackedImage.referenceImage.name;
                        readItemObject = arObject;
                    }
                    else if (readItemName == trackedImage.referenceImage.name)
                    {
                        arObject.transform.SetPositionAndRotation(imageMarkerTransform.transform.position, markerFrontRotationTmp);
                        
                    }
                    else if(readItemName != trackedImage.referenceImage.name)
                    {
                        readItemObject.SetActive(false);

                        targetList.Clear();

                        if (Menuselect.scene == 7)
                        {
                            targetList.AddRange(ps.SGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 4)
                        {
                            targetList.AddRange(ps.HGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 5)
                        {
                            targetList.AddRange(ps.ThreeGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 6)
                        {
                            targetList.AddRange(ps.FourGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 12)
                        {
                            targetList.AddRange(ps.ATwoGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 13)
                        {
                            targetList.AddRange(ps.AThreeGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }
                        else if (Menuselect.scene == 14)
                        {
                            targetList.AddRange(ps.AFourGetTargetListFromMainItemNameWithRank(trackedImage.referenceImage.name));
                        }

                        //認識した場所にオブジェクトをセット
                        markerFrontRotation.x = 0;
                        markerFrontRotation.z = 0;
                        markerFrontRotation.y = 0;
                        arObject.transform.SetPositionAndRotation(imageMarkerTransform.transform.position, markerFrontRotation);

                        markerFrontRotationTmp = markerFrontRotation;
                        readItemName = trackedImage.referenceImage.name;
                        readItemObject = arObject;
                        readItemObject.SetActive(true);
                    }
                    
                    UArobjectSetItemInfo(arObject, targetList, textAttributionList, dropdown.options[dropdown.value].text);

                    //辞書を作る 画像の名前とARオブジェクトのPrefabを紐づける
                    for (var i = 0; i < _arPrefabs.Length; i++)
                    {
                        if (_imageManager.referenceLibrary[i].name == trackedImage.referenceImage.name)
                            _markerNameAndPrefabDictionary.Add(_imageManager.referenceLibrary[i].name, arObject);
                    }
                }
                
            }
        }
        else if(trackedImage.trackingState == TrackingState.Limited) //一部：Limited、何もない：None
        {
        }

    }

    void Update()
    {
        if (readItemName != "")
        {
            UArobjectSetItemInfo(readItemObject, targetList, textAttributionList, dropdown.options[dropdown.value].text);

            //辞書を作る 画像の名前とARオブジェクトのPrefabを紐づける
            for (var i = 0; i < _arPrefabs.Length; i++)
            {
                if (_imageManager.referenceLibrary[i].name == readItemName)
                    _markerNameAndPrefabDictionary.Add(_imageManager.referenceLibrary[i].name, readItemObject);
                
            }
        }
    }

    public void ObjectTextChange()
    {
        bottonCount += 1;
        if(textState == 2 || textState == 1)
        {
            textState = 0;
        }
        else if(textState == 0)
        {
            textState = 2;
        }
    }
    public void KanrenChange()
    {
        if (kanrenState == 2)
        {
            kanrenState = 1;
            pageChangeButton.GetComponentInChildren<TextMeshProUGUI>().text = "1/2";
            
        }
        else if (kanrenState == 1)
        {
            kanrenState = 2;
            pageChangeButton.GetComponentInChildren<TextMeshProUGUI>().text = "2/2";
        }
    }

    /// <summary>
    /// TrackedImagesChanged時の処理
    /// </summary>
    /// <param name="eventArgs">検出イベントに関する引数</param>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            ActivateARObject(trackedImage);
        }
    }
    
    //関連商品の切り替えに対応
    public void UArobjectSetItemInfo(GameObject arObject, List<Item> targetList, List<int> textAttributionList, string attributinText)
    {
        TextMeshProUGUI obtext;
        RawImage rawImage;

        //オブジェクトの関連商品情報をリセット
        for (int i = 1; i < 4; i++)
        {
            //商品情報と商品画像
            rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 2).GetComponent<RawImage>();
            obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
            rawImage.texture = Resources.Load<Texture2D>("ItemImage/ClearImage");
            obtext.SetText("");

            //ランク
            rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
            rawImage.texture = Resources.Load<Texture2D>("ItemImage/ClearImage");

            //商品名
            obtext = arObject.transform.GetChild(0).transform.GetChild(i + 15).GetComponent<TextMeshProUGUI>();
            obtext.SetText("");
        }
        

        if (kanrenState == 1)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                if (i == 0)
                {
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(TItemTextOutput(targetList[i], textAttributionList, attributinText));
                }
                else if (i == 4)
                {
                    break;
                }
                else
                {
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 2).GetComponent<RawImage>();
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(TItemTextOutput(targetList[i], textAttributionList, attributinText));
                    rawImage.texture = Resources.Load<Texture2D>("ItemImage/" + targetList[i].GetItemName());
                }
                rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
                rawImage.texture = Resources.Load<Texture2D>("Rank/Rank" + targetList[i].GetRank());
                obtext = arObject.transform.GetChild(0).transform.GetChild(i + 15).GetComponent<TextMeshProUGUI>();
                obtext.SetText(targetList[i].GetItemName());
            }
        }
        else
        {
            
            for (int i = 0; i < targetList.Count; i++)
            {
                if (i == 0)
                {
                    //商品情報
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(TItemTextOutput(targetList[i], textAttributionList, attributinText));

                    //ランク
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
                    rawImage.texture = Resources.Load<Texture2D>("Rank/Rank" + targetList[i].GetRank());

                    //商品名
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 15).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(targetList[i].GetItemName());
                }
                else if (i == 4)
                {
                    break;
                }
                else
                {
                    //商品情報と商品画像
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 2).GetComponent<RawImage>();
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(TItemTextOutput(targetList[i + 3], textAttributionList, attributinText));
                    rawImage.texture = Resources.Load<Texture2D>("ItemImage/" + targetList[i + 3].GetItemName());

                    //ランク
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
                    rawImage.texture = Resources.Load<Texture2D>("Rank/Rank" + targetList[i + 3].GetRank());

                    //商品名
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 15).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(targetList[i + 3].GetItemName());
                }
            }
        }
        
    }
    
    public string TItemTextOutput(Item item, List<int> textAttributionList, string bottontext)
    {
        string itemText = "";
        string attributionText = "";
        for (int i = 0; i < textAttributionList.Count; i++)
        {
            attributionText = GetDropdownText(textAttributionList[i]);
            if (attributionText == bottontext)
            {
                itemText += "<color=blue>";
            }

            if(textAttributionList[i] == 1)
            {
                itemText += "価格：" + item.GetPrice() + "円";
            }
            else if (textAttributionList[i] == 2)
            {
                itemText += "内容量：" + item.GetVolume();
            }
            else if (textAttributionList[i] == 3)
            {
                itemText += "用量：" + item.GetUsage();
            }
            else if (textAttributionList[i] == 4)
            {
                itemText += "成分：" + item.GetEfficacy();
            }
            else if (textAttributionList[i] == 5)
            {
                itemText += "メーカ：" + item.GetMaker();
            }
            else if (textAttributionList[i] == 6)
            {
                itemText += "形状：" + item.GetForm();
            }

            if (attributionText == bottontext)
            {
                itemText += "</color>\n";
            }
            else
            {
                itemText += "\n";
            }
        }
        
        return itemText;
    }
    
    public string GetDropdownText(int TextNumber)
    {
        string text="";
        if(TextNumber == 1)
        {
            text = "価格";
        }
        else if (TextNumber == 2)
        {
            text = "内容量";
        }
        else if (TextNumber == 3)
        {
            text = "用量";
        }
        else if(TextNumber == 4)
        {
            text = "成分";
        }
        else if (TextNumber == 5)
        {
            text = "メーカ";
        }
        else if (TextNumber == 6)
        {
            text = "形状";
        }

        return text;
    }
    public void attribution1ValueChange()
    {
        textAttributionList[0] = attributionDropdown1.value;
        dropdown.options[1].text = GetDropdownText(attributionDropdown1.value);
    }
    public void attribution2ValueChange()
    {
        textAttributionList[1] = attributionDropdown2.value;
        dropdown.options[2].text = GetDropdownText(attributionDropdown2.value);
    }
    public void attribution3ValueChange()
    {
        textAttributionList[2] = attributionDropdown3.value;
        dropdown.options[3].text = GetDropdownText(attributionDropdown3.value);
    }
}
