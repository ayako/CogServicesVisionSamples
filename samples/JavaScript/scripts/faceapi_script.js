$(function () {

    var uploadedImage;

    // 画像を画面に表示
    var showImage = function () {
        if (uploadedImage) {
            var blobUrl = window.URL.createObjectURL(uploadedImage);
            $("#ImageToAnalyze").attr("src", blobUrl);
        }
    };

    //画像の分析    
    var getFaceInfo = function () {

        // Face API の Subscription Key と URL をセット
        // サブスクリプション画面に表示される URL および Key をコピーしてください
        var subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
        var endpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/face/v1.0";
        
        // Face API 呼び出し URL をセット
        var webSvcUrl = endpoint + "/detect?returnFaceAttributes=emotion";       

        // 画面に表示するメッセージをセット
        var outputDiv = $("#OutputDiv");
        if(document.getElementById('imageFile').value == "")
        {
            // 初期設定
            outputDiv.text("画像を選択してください");
        }
        else{
            // 画像分析中
            outputDiv.text("分析中...");
        }
    
        // Face API を呼び出すためのパラメーターをセットして呼び出し
        var xmlHttp = new XMLHttpRequest();
        xmlHttp.open("POST", webSvcUrl, true);
        xmlHttp.setRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        xmlHttp.setRequestHeader("Content-Type", "application/octet-stream");
        xmlHttp.send(uploadedImage);
        xmlHttp.onreadystatechange = function () {

            // データが取得出来た場合
            if (this.readyState == 4 && this.status == 200) {

                if (this.responseText.length > 2)
                {
                    let data = JSON.parse(this.responseText)
                    // 検出された顔の表示位置を取得
                    var faceRectange = data[0].faceRectangle;
                    var faceWidth = faceRectange.width;
                    var faceHeight = faceRectange.height;
                    var faceLeft = faceRectange.left;
                    var faceTop = faceRectange.top;

                    // 画面に描画
                    $("#Rectangle").css("top", faceTop);
                    $("#Rectangle").css("left", faceLeft);
                    $("#Rectangle").css("height", faceHeight);
                    $("#Rectangle").css("width", faceHeight);
                    $("#Rectangle").css("display", "block");

                    //小数点6位までを残す関数 (表情スコアの丸めに利用)
                    function floatFormat( number ) {
                        return Math.round( number * Math.pow( 10 , 6 ) ) / Math.pow( 10 , 6 ) ;
                    }

                    // 検出された表情スコアを取得
                    var faceScore = data[0].faceAttributes.emotion;
                    var faceAnger = floatFormat(faceScore.anger);
                    var faceContempt =  floatFormat(faceScore.contempt);
                    var faceDisgust = floatFormat(faceScore.disgust);
                    var faceFear = floatFormat(faceScore.fear);
                    var faceHappiness = floatFormat(faceScore.happiness);
                    var faceNeutral = floatFormat(faceScore.neutral);
                    var faceSadness = floatFormat(faceScore.sadness);
                    var faceSurprise = floatFormat(faceScore.surprise);                

                    // 表情スコアを表示
                    var outputText = "";
                    outputText += "<h3>" + "結果:" + "</h3>";
                    outputText += "怒り　　: " + faceAnger + "<br>";
                    outputText += "軽蔑　　: " + faceContempt + "<br>";
                    outputText += "ムカつき: " + faceDisgust + "<br>";
                    outputText += "恐れ　　: " + faceFear + "<br>";
                    outputText += "喜び　　: " + faceHappiness + "<br>";
                    outputText += "無表情　: " + faceNeutral + "<br>";
                    outputText += "悲しみ　: " + faceSadness + "<br>";
                    outputText += "驚き　　: " + faceSurprise + "<br>";

                    outputDiv.html(outputText);

                }
                // データが空だった場合
                else {
                    outputDiv.text("検出できませんでした");
                };
            }
            else
            // データが取得できなかった場合
            {
                outputDiv.text("ERROR!");
            };
        };
    };

    // 表示するものがない場合
    var hideMarkers = function () {
        $("#Rectangle").css("display", "none");
    };

    // 画像が変更された場合（再度分析＆表示)
    $("#imageFile").on('change', function(e){
        uploadedImage = e.target.files[0];
        hideMarkers();
        showImage();
        getFaceInfo();
    });

});