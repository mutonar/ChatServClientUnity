using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StartConnection : MonoBehaviour
{
    [SerializeField] InputField _sendData;
    [SerializeField] Text textData;
    [SerializeField] Dropdown dropdown;
    [SerializeField] Scrollbar _scrollbar;
    [SerializeField] Button _ButtonEnter;
    private Program program = new Program();
    private Text _fromServer;
    private BuferData _BuferData = null;
    private bool _userSet = false;
    private bool _colorCasedR = true;


    // Start is called before the first frame update
    void Start()
    {
        textData.text += "We are <color=#FF0000>colorfully</color> with envy";
        
        dropdown.onValueChanged.AddListener(delegate { // слашатель изменения  как добавить просто активность
            CaseColor(dropdown.value);
        });


    }

    private void CaseColor(int vaCaseColor)
    {
        _colorCasedR = false;
        _BuferData._dataToSocket = vaCaseColor.ToString();
        dropdown.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            sendMessageToServ();
        }

        if (_BuferData != null && _BuferData._dataFromSocket != null) {
            
            if (_colorCasedR == true && _BuferData._dataFromSocket.Contains("Color:")) //пока так ловим цвет и преобразуем
            {
                List<Color> listColorServer = parseColor(_BuferData._dataFromSocket);

                dropdown.gameObject.SetActive(true);
                dropdown.ClearOptions();
                dropdown.captionText.text = "Get Color";
                for (int i = 0; i < listColorServer.Count; i++)
                {
                    string nameColor = ColorUtility.ToHtmlStringRGBA(listColorServer[i]);
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = nameColor;

                    var texture = new Texture2D(100, 100);
                    for (int w = 0; w < texture.width; w++)
                    {
                        for (int h = 0; h < texture.height; h++)
                        {
                            texture.SetPixel(w, h, listColorServer[i]);
                        }
                    }
                    
                    texture.Apply();
                    var item = new Dropdown.OptionData(nameColor, Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero)); // creating dropdown item and converting texture to sprite
                    dropdown.options.Add(item);
                }
            }
            
            if (_userSet == false && _BuferData._dataFromSocket.Contains("Enter username:"))
            {
                _userSet = true;
            }
            int countRow = _BuferData._dataFromSocket.Split('\n').Length;
            textData.text += _BuferData._dataFromSocket;
            var rtText = textData.GetComponent<RectTransform>();
            rtText.sizeDelta = new Vector2(rtText.sizeDelta.x , rtText.sizeDelta.y + (5 * countRow));
            _scrollbar.value = 0;

            _BuferData._dataFromSocket = null;

        }
    }

    public void connectServer()
    {
        _BuferData = new BuferData();
        Program program = new Program();
        textData.text = "";
        textData.text += program.ConnectServer("5.17.3.161", 11000, _BuferData);
        //textData.text += "\n" +  program.ConnectServer("127.0.0.1", 11000, _BuferData);
        _ButtonEnter.gameObject.SetActive(false);

    }
    public void sendMessageToServ() {
       
        if (_sendData.text != null && _sendData.text.Equals("") == false)
        {
            
         /*   if (_colorCasedR == false & _userSet)  // че за лажа с этим условием?
            {
                CaseColor(dropdown.value);
                _userSet = false;
            }
            */
            _BuferData._dataToSocket = _sendData.text;
        }
    }

    private List<Color> parseColor(string dataColor)
    {// первоначальное получение цвета
        List<Color> listColor = new List<Color>();
        
        string[] splitDataColor = dataColor.Split('\n');
        for (int i = 1; i < splitDataColor.Length; i++)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(splitDataColor[i], out color))
            { listColor.Add(color); }
        }

        return listColor;
    }

}
