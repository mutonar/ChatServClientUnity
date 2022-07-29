using System;
using System.Collections;
using System.Collections.Generic;

public class BuferData 
{
    private string dataFromSocket;
    public string _dataFromSocket 
    {
        get => parsDataFromServ(dataFromSocket);
        set => dataFromSocket = value;
    }
    public string _dataToSocket { get; set; }

    private string parsDataFromServ(string str) {
        string _textPars = null;
        if (str != null)
        {
            string[] splitTxt = str.Split(new[] { "::" }, StringSplitOptions.None);
            if (splitTxt.Length == 3)
            {

                _textPars = "<color=" + splitTxt[1] + ">" + splitTxt[0] + ":" + splitTxt[2] + "</color>";
                return _textPars;
            }
            else
            {
                return str;
            }
        }
        return str;
    }
}
