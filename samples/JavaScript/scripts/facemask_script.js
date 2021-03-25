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
        var endpoint = "https://YOUR_ENDPOINT/";
        
        // Face API 呼び出し URL をセット
        var webSvcUrl = endpoint + "face/v1.0/detect?returnFaceAttributes=mask&detectionModel=detection_03";       

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
                    $("#Rectangle").css("width", faceWidth);
                    $("#Rectangle").css("display", "block");

                    // 検出された結果を取得
                    var maskResult = data[0].faceAttributes.mask;

                    // maskType と noseAndMouthCovered の値を設定
                    var outputText = "";
                    outputText += "<h3>" + "結果:" + "</h3>";
                    outputText += "<table>";
                    outputText += "<tr><td>Mask Type</td><td>:</td><td>" + maskResult.type + "</td></tr>";
                    outputText += "<tr><td>Nose and Mask Covered</td><td>:</td><td>" + maskResult.noseAndMouthCovered + "</td></tr>";
                    outputText += "</table><br/>";

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
