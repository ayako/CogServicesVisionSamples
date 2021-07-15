$(function () {

    var uploadedImage;
    var outputDiv = $("#OutputDiv");

    // 画像を画面に表示
    var showImage = function () {
        if (uploadedImage) {
            var blobUrl = window.URL.createObjectURL(uploadedImage);
            $("#ImageToAnalyze").attr("src", blobUrl);
        }
    };
    var resultUrl = "";


    // Form Recognizer の Subscription Key と URL をセット
    // Azure Portal 画面に表示される URL および Key をコピーしてください
    var subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
    var endpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/";
    
    // Form Recognizer の Model Id をセット
    // FOTT ツールに表示される Model Id をコピーしてください
    var modelId = "YOUR_MODEL_ID";
        
    // Form Recognizer 呼び出し URL をセット
    var webSvcUrl = endpoint + "formrecognizer/v2.1/custom/models/" + modelId + "/analyze";

    //画像の分析    
    var postFormImage = function () {

        // 画面に表示するメッセージをセット
        if(document.getElementById('imageFile').value == "")
        {
            // 初期設定
            outputDiv.text("画像を選択してください");
        }
        else{
            // 画像表示
            outputDiv.text("画像を表示します");
        };
    
        // Form Recognizer を呼び出すためのパラメーターをセットして Post
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", webSvcUrl, true);
        xmlHttp.setRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        xmlHttp.setRequestHeader("Content-Type", "application/octet-stream");
        xmlHttp.send(uploadedImage);
        xmlHttp.onreadystatechange = function () {

            // データの POST が成功した場合
            if (this.status == 202) {
                // 結果取得URL を取得
                resultUrl = this.getResponseHeader("operation-location");
                outputDiv.text("[結果の取得] をクリックしてください");
            }
            else
            // データの POST が失敗した場合
            {
                outputDiv.text("ERROR!: failed to send image file.");
            };
        };
    };

    var getFormInfo = function () {

        // Form Recognizer から認識結果を取得
        var xmlHttp2 = new XMLHttpRequest();
        xmlHttp2.open("GET", resultUrl, true);
        xmlHttp2.setRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        xmlHttp2.send(null);
        xmlHttp2.onreadystatechange = function () {

            // データが取得できた場合
            if (this.status == 200) 
            {
                var data = JSON.parse(this.responseText)

                // 分析結果が取得できた場合
                if (data.status == "succeeded")
                {
                    function trimFormat(str) { return str.replace(/\s+/g, "");};

                    var fieldKeys = Object.keys(data.analyzeResult.documentResults[0].fields);

                    var outputText = "<h3>" + "結果:" + "</h3>";
                    outputText += "<table>";
                    fieldKeys.forEach(element => { 
                        outputText += "<tr><td>" + element + ":</td><td>"
                                        + trimFormat(data.analyzeResult.documentResults[0].fields[element].valueString) +"</td></tr>";                        
                    });
                    outputText += "</table>";
                    outputDiv.html(outputText);                
                }
                else
                {
                    outputDiv.text("分析中です。しばらくしてから再度 [結果の取得] をクリックしてください");
                }
 
            }
            // データにアクセスできない場合
            else if (this.status != 200)
            {
                outputDiv.text("ERROR!: failed to get result.");
            }

            else
            {
                outputDiv.text("ERROR!: failed to send request.");
            }
        };
    };

    // 画像が変更された場合（再度分析＆表示)
    $("#imageFile").on('change', function(e){
        uploadedImage = e.target.files[0];
        showImage();
        postFormImage();
    });

    // 結果取得＆表示
    $("#getResult").on('click', function(e){
        getFormInfo();
    });

});