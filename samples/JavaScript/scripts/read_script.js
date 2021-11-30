$(function () {

    var outputMsgDiv = $("#OutputMsgDiv");
    var outputResultDiv = $("#OutputResultDiv");
    var resultUrlDiv = $("#ResultUrlDiv");

    // Computer Vision API の Subscription Key と URL をセット
    // サブスクリプション画面に表示される URL および Key をコピーしてください
    var subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
    var endpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/";

    // Computer Vision API 呼び出し URL をセット
    var webSvcUrl = endpoint + "vision/v3.2/read/analyze";       

    var uploadedImage;

    // 画像を画面に表示
    var showImage = function () {
        if (uploadedImage) {
            var blobUrl = window.URL.createObjectURL(uploadedImage);
            $("#ImageToAnalyze").attr("src", blobUrl);
        }
        outputMsgDiv.text("[Analyze] をクリックしてデータを送信してください");
    };

    //画像の分析    
    var postImage = function () {

        // 画面に表示するメッセージをセット
        if(document.getElementById('imageFile').value == "")
        {
            // 初期設定
            outputMsgDiv.text("画像を選択してください");
        }
        else{
            // 画像分析中
            outputMsgDiv.text("送信中...");
        }
    
        // Computer Vision API を呼び出すためのパラメーターをセットして呼び出し
        fetch( webSvcUrl, {
            method: 'POST',
            mode: 'cors',
            headers: {
                'Ocp-Apim-Subscription-Key': subscriptionKey,
                'Content-Type': 'application/octet-stream'
            },
            body: uploadedImage

        }).then((res)=>{

            outputMsgDiv.text("On process: posting data...");

            if (!res.ok) {
                outputMsgDiv.text("ERROR!: failed to send text data.");
            }
            return(res.headers.get('operation-location'));

        }).then((resultUrl)=>{
                // 結果取得URL を取得
                resultUrlDiv.text(resultUrl);
                outputMsgDiv.text("データを送信しました。結果を取得するには [Result] をクリックしてください");
        });

    };

    // 結果の取得
    var getResult = function () {

        outputMsgDiv.text("On process: checking analysis task status...");

        var resultUrl = document.getElementById('ResultUrlDiv').textContent;
        fetch( resultUrl, {
                method: 'GET',
                mode: 'cors',
                headers: {
                    'Ocp-Apim-Subscription-Key': subscriptionKey
                }
        }).then((res)=>{

            if (!res.ok) {
                outputMsgDiv.text("ERROR!: failed to access to result.");
            }
            return(res.json() );

        }).then((json)=>{

            // 分析結果が取得できた場合
            if (json.status == "succeeded")
            {
                var outputResultText = "";
                json.analyzeResult.readResults[0].lines.forEach(line => {
                    outputResultText += line.text + "<br>";
                });;

                outputMsgDiv.text("結果表示");
                outputResultDiv.html(outputResultText);                
            }
            else
            {
                outputMsgDiv.text("分析中です。しばらくしてから再度 [Result] をクリックしてください");
            }
        });

    };


    // 画像が変更された場合
    $("#imageFile").on('change', function(e){
        uploadedImage = e.target.files[0];
        showImage();
        outputResultDiv.html(null);                
    });

    // データの送信
    $("#PostImage").on('click', function(e){
        postImage();
    });

    // 結果取得＆表示
    $("#GetResult").on('click', function(e){
        getResult();
    });
    

});
