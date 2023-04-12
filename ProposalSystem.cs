using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ProposalSystem : MonoBehaviour
{
    /// <summary>
    /// �}�[�J�[�p�I�u�W�F�N�g�̃v���n�u
    /// </summary>
    [SerializeField] public GameObject[] _arPrefabs;

    /// <summary>
    /// ARTrackedImageManager
    /// </summary>
    [SerializeField] public ARTrackedImageManager _imageManager;

    /// <summary>
    /// �}�[�J�[�p�I�u�W�F�N�g�̃v���n�u�ƕ������R�Â�������
    /// </summary>
    /// readonly
    public Dictionary<string, GameObject> _markerNameAndPrefabDictionary = new Dictionary<string, GameObject>();

    public static int textState = 1;

    public static int bottonCount = 1;

    public ARSessionOrigin aRSessionOrigin;

    Quaternion markerFrontRotationTmp;

    //Dropdown���i�[����ϐ� �����\���̃h���b�v�_�E��
    [SerializeField] private TMP_Dropdown dropdown;
    
    //���i�����̃h���b�v�_�E��1
    [SerializeField] private TMP_Dropdown attributionDropdown1;
    //���i�����̃h���b�v�_�E��1
    [SerializeField] private TMP_Dropdown attributionDropdown2;
    //���i�����̃h���b�v�_�E��1
    [SerializeField] private TMP_Dropdown attributionDropdown3;

    [SerializeField] private Button pageChangeButton;

    //�@�A�C�e���f�[�^�x�[�X
    [SerializeField]
    private ItemDataBase itemDataBase;

    //�^�[�Q�b�g���X�g�����Ŏg�����X�g
    public List<Item> targetList;

    //�Y�����鏤�i�̖��O�����̃��X�g
    private List<string> correspondItemList;

    //�\�����鏤�i�̑���
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
        //�Y�����鏤�i�̏�����
        targetList = new List<Item>();
        
        ps = new psSQL();
        ps.FirstConnect();
        
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        //��������� �摜�̖��O��AR�I�u�W�F�N�g��Prefab��R�Â���
        for (var i = 0; i < _arPrefabs.Length; i++)
        {
            var arPrefab = Instantiate(_arPrefabs[i]);
            _markerNameAndPrefabDictionary.Add(_imageManager.referenceLibrary[i].name, arPrefab);
            arPrefab.SetActive(false);
        }
        
        //�v���_�E�����j���[�̃e�L�X�g�Z�b�g
        for (int i = 0; i < textAttributionList.Count; i++)
        {
            dropdown.options[i + 1].text = GetDropdownText(textAttributionList[i]);
        }
        
        if (Menuselect.scene == 4)
        {
            correspondItemList = new List<string>() { "�Z���x�[�����ݏ� 12��", "���O���ݒ���v���X ���� 50��" };
        }
        else if (Menuselect.scene == 5)
        {
            correspondItemList = new List<string>() { "�X�N���[�g�ݒɖ� 36��", "�u�X�R�p��A�� 20��","�T�N����Q 6��" };
        }
        else if (Menuselect.scene == 6)
        {
            correspondItemList = new List<string>() { "�R�[���b�N 120��", "�A�N�A�i�`�������֔�� 132��", "3A�}�O�l�V�A 90��","�X���[���b�N�}�O�l�V�E�� 30��" };
        }
        else if(Menuselect.scene == 7)
        {
            correspondItemList = new List<string>() { "VWX", "ABC" };
        }
        else if (Menuselect.scene == 12)
        {
            correspondItemList = new List<string>() { "�o�C�G���A�X�s���� 30��", "�V�Z�f�X�� 40��" };
        }
        else if (Menuselect.scene == 13)
        {
            correspondItemList = new List<string>() { "�C�uA�� 90��", "���L�m����M���ɖ� 60��", "�N�C�b�N���ɖ�DX 20��" };
        }
        else if (Menuselect.scene == 14)
        {
            correspondItemList = new List<string>() { "�m�[�V���A�Z�g�A�~�m�t�F�� 48��", "�^�C���m�[��A 20��", "�����O��N 20��", "�i������ 48��" };
        }
        pageChangeButton.GetComponentInChildren<TextMeshProUGUI>().text = Menuselect.scene.ToString();
        
    }

    private void OnDisable()
    {
        _imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //�^�[�Q�b�g�̏�����
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
    /// �F�������摜�}�[�J�[�ɉ����ĕR�Â���AR�I�u�W�F�N�g��\��
    /// </summary>
    /// <param name="trackedImage">�F�������摜�}�[�J�[</param>
    private void ActivateARObject(ARTrackedImage trackedImage)
    {
        //�F�������摜�}�[�J�[�̖��O���g���Ď�������C�ӂ̃I�u�W�F�N�g����������o��
        var arObject = _markerNameAndPrefabDictionary[trackedImage.referenceImage.name];
        var imageMarkerTransform = trackedImage.transform;

        //�ʒu���킹
        if (Menuselect.scene < 3 && Menuselect.scene != 1)
        {
            Vector3 pos = imageMarkerTransform.transform.position;
            pos.y += 0.04f;
            imageMarkerTransform.transform.position = pos;
        }

        //AR�I�u�W�F�N�g���h��錴�����������H
        //�摜��F�������Ƃ�
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            
            //�F���������̂��Y�����鏤�i���ǂ����̔���
            if (correspondItemList.Contains(trackedImage.referenceImage.name))
            {
                //�F�������摜�̈ʒu���r���[�|�[�g���W�ɕϊ��i��ʍ�������(0,0)�A�E���(1,1)�Ƃ��āj
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
                        

                        //�F�������ꏊ�ɃI�u�W�F�N�g���Z�b�g
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

                        //�F�������ꏊ�ɃI�u�W�F�N�g���Z�b�g
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

                    //��������� �摜�̖��O��AR�I�u�W�F�N�g��Prefab��R�Â���
                    for (var i = 0; i < _arPrefabs.Length; i++)
                    {
                        if (_imageManager.referenceLibrary[i].name == trackedImage.referenceImage.name)
                            _markerNameAndPrefabDictionary.Add(_imageManager.referenceLibrary[i].name, arObject);
                    }
                }
                
            }
        }
        else if(trackedImage.trackingState == TrackingState.Limited) //�ꕔ�FLimited�A�����Ȃ��FNone
        {
        }

    }

    void Update()
    {
        if (readItemName != "")
        {
            UArobjectSetItemInfo(readItemObject, targetList, textAttributionList, dropdown.options[dropdown.value].text);

            //��������� �摜�̖��O��AR�I�u�W�F�N�g��Prefab��R�Â���
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
    /// TrackedImagesChanged���̏���
    /// </summary>
    /// <param name="eventArgs">���o�C�x���g�Ɋւ������</param>
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
    
    //�֘A���i�̐؂�ւ��ɑΉ�
    public void UArobjectSetItemInfo(GameObject arObject, List<Item> targetList, List<int> textAttributionList, string attributinText)
    {
        TextMeshProUGUI obtext;
        RawImage rawImage;

        //�I�u�W�F�N�g�̊֘A���i�������Z�b�g
        for (int i = 1; i < 4; i++)
        {
            //���i���Ə��i�摜
            rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 2).GetComponent<RawImage>();
            obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
            rawImage.texture = Resources.Load<Texture2D>("ItemImage/ClearImage");
            obtext.SetText("");

            //�����N
            rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
            rawImage.texture = Resources.Load<Texture2D>("ItemImage/ClearImage");

            //���i��
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
                    //���i���
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(TItemTextOutput(targetList[i], textAttributionList, attributinText));

                    //�����N
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
                    rawImage.texture = Resources.Load<Texture2D>("Rank/Rank" + targetList[i].GetRank());

                    //���i��
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 15).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(targetList[i].GetItemName());
                }
                else if (i == 4)
                {
                    break;
                }
                else
                {
                    //���i���Ə��i�摜
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 2).GetComponent<RawImage>();
                    obtext = arObject.transform.GetChild(0).transform.GetChild(i + 6).GetComponent<TextMeshProUGUI>();
                    obtext.SetText(TItemTextOutput(targetList[i + 3], textAttributionList, attributinText));
                    rawImage.texture = Resources.Load<Texture2D>("ItemImage/" + targetList[i + 3].GetItemName());

                    //�����N
                    rawImage = arObject.transform.GetChild(0).transform.GetChild(i + 11).GetComponent<RawImage>();
                    rawImage.texture = Resources.Load<Texture2D>("Rank/Rank" + targetList[i + 3].GetRank());

                    //���i��
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
                itemText += "���i�F" + item.GetPrice() + "�~";
            }
            else if (textAttributionList[i] == 2)
            {
                itemText += "���e�ʁF" + item.GetVolume();
            }
            else if (textAttributionList[i] == 3)
            {
                itemText += "�p�ʁF" + item.GetUsage();
            }
            else if (textAttributionList[i] == 4)
            {
                itemText += "�����F" + item.GetEfficacy();
            }
            else if (textAttributionList[i] == 5)
            {
                itemText += "���[�J�F" + item.GetMaker();
            }
            else if (textAttributionList[i] == 6)
            {
                itemText += "�`��F" + item.GetForm();
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
            text = "���i";
        }
        else if (TextNumber == 2)
        {
            text = "���e��";
        }
        else if (TextNumber == 3)
        {
            text = "�p��";
        }
        else if(TextNumber == 4)
        {
            text = "����";
        }
        else if (TextNumber == 5)
        {
            text = "���[�J";
        }
        else if (TextNumber == 6)
        {
            text = "�`��";
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
