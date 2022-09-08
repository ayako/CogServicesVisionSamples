# Microsoft Azure Cognitive Services | Applied AI Services を利用した 画像分析アプリ (202111 アップデート)

"人工知能 API" [Microsoft Azure Cognitive Services](https://www.microsoft.com/cognitive-services/) や [Microsoft Azure Applied AI Services](https://azure.microsoft.com/ja-jp/product-categories/applied-ai-services/) を使うと、画像分析を行うエンジンをノーコーディングで利用、作成できます。

- [Face API](https://azure.microsoft.com/ja-jp/services/cognitive-services/face/) は画像から人間の顔を検出し、分析するエンジンがすぐに Web API で利用できます。
- [Computer Vision](https://azure.microsoft.com/ja-jp/services/cognitive-services/computer-vision/) は画像分析(物体検出＆分析、OCR、タグ＆キャプション付与 など) を行うエンジンを学習などの作業不要なく Web API で利用できます。
- [Custom Vision Service](https://azure.microsoft.com/ja-jp/services/cognitive-services/custom-vision-service/) は、ご自分で用意した画像をアップロードしてタグ付け、学習させることで、画像の分類 (Classification) や 画像に写っているモノの抽出 (Object Detection) を行うエンジンを簡単に作成でき、Web API として利用できます。また TensorFlow / CoreML / ONNX、または Docker コンテナー向けに Export して利用することもできます。
- [Form Recognizer](https://azure.microsoft.com/ja-jp/services/form-recognizer/) は帳票、IDなどの定型フォーム画像を読み取るエンジンを作成、すぐに Web API で利用できるサービスです。

# サンプルの動作確認

- [Face API (Emotion)](https://cogservicesvisionsamples.azurewebsites.net/Face)
- [Face API (Mask Recognition)](https://cogservicesvisionsamples.azurewebsites.net/MaskRecognition)
- [Computer Vision (Read)](https://cogservicesvisionsamples.z11.web.core.windows.net/Read.html)
- [Custom Vision (Classification)](https://cogservicesvisionsamples.azurewebsites.net/CustomVisionClassification)
- [Form Recognizer](https://cogservicesvisionsamples.azurewebsites.net/FormRecognition)

![](doc_images/CognitiveAppSampleJS.png)
![](doc_images/CognitiveAppSample202103.png)
![](doc_images/CognitiveAppSample202111.png)

# サンプルの利用方法

- Face API ([C#](#c) | [HTML/JavaScript](#htmljavascript))
- Computer Vision ([HTML/JavaScript](#htmljavascript-1))
- Custom Vision ([C#](#c-1) | [HTML/JavaScript](#htmljavascript-2))
- Custom Vision (model export) ([ONNX & UWP(C#)](#onnx--uwp))
- Form Recognizer ([C#](#c-2) | [HTML/JavaScript](#htmljavascript-3)))

## Face API

Face API の エンドポイント(URL) と キー (Subscription Key) にご自分のサブスクリプションの情報を入力します。
**Key1** に表示されている文字列が キー (Subscription Key) になります。

<img src="doc_images/face01.png" width="600">


### C#

[Face.cshtml.cs](samples/CSharp/CogServicesVisionSamples_202107/Pages/Face.cshtml.cs)
[MaskRecognition.cshtml.cs](samples/CSharp/CogServicesVisionSamples_202107/Pages/MaskRecognition.cshtml.cs)

```Face.cshtml.cs
// Setting for using Face API 
private const string faceSubscriptionKey = "YOUR_SUBSCRIPTION_KEY";
private const string faceEndpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com";
```

Visual Studio プロジェクトを開き、NuGet パッケージの復元を行います。ビルド＆起動して、localhost:<YOUR_PORT>/Face (Emotion) または localhost:<YOUR_PORT>/MaskRecognition にアクセスし、画像をアップロードして動作を確認できます。

### HTML/JavaScript

[faceapi_script.js](samples/JavaScript/scripts/faceapi_script.js)
[facemask_script.js](samples/JavaScript/scripts/facemask_script.js)

```faceapi_script.js
// Face API の Subscription Key と URL をセット
// サブスクリプション画面に表示される URL および Key をコピーしてください
var subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
var endpoint = "https://YOUR_ENDPOINT/";
```

FaceAPI.html を開き、画像をアップロードして動作を確認できます。


## Computer Vision

Computer Vision API の エンドポイント(URL) と キー (Subscription Key) にご自分のサブスクリプションの情報を入力します。
**Key1** に表示されている文字列が キー (Subscription Key) になります。

<img src="doc_images/computervision01.png" width="600">

### HTML/JavaScript

[read_script.js](samples/JavaScript/scripts/read_script.js)

```read_script.js
// Computer Vision API の Subscription Key と URL をセット
// サブスクリプション画面に表示される URL および Key をコピーしてください
var subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
var endpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/";
```

Read.html を開き、画像をアップロードして動作を確認できます。


## Custom Vision

Custom Vision のエンドポイント(URL) と キー (Prediction Key) にご自分のサブスクリプション および 作成した Custom Vision App の情報を入力します。

### C#

![](doc_images/customvision11.png)
![](doc_images/customvision12.png)

[CustomVisionClassification.cshtml.cs](samples/CSharp/CogServicesVisionSamples_201906/Pages/CustomVisionClassicifation.cshtml.cs) </br>
[CustomVisionDetection.cshtml.cs](samples/CSharp/CogServicesVisionSamples_201906/Pages/CustomVisionDetection.cshtml.cs)

```CustomVisionClassification.cshtml.cs, CustomVisionDetection.cshtml.cs
// Setting for using Custom Vision 
private const string cvPredictionKey = "YOUR_CUSTOMVISION_PREDICTION_KEY";
private const string cvEndpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com";
private const string cvProjectId = "YOUR_CUSTOMVISION_PROJECTID";
private const string cvPublishName = "YOUR_CUSTOMVISION_PROJECT_PUBLISHNAME";//"Iteration1"
```

Visual Studio プロジェクトを開き、NuGet パッケージの復元を行います。ビルド＆起動して、localhost:<YOUR_PORT>/CustomVisionClassiication または CustomVisionDetection にアクセスし、画像をアップロードして動作を確認できます。


### HTML/JavaScript

![](doc_images/customvision01.png)

[customvision_script.js](samples/JavaScript/scripts/customvision_script.js)

```customvision_script.js
// Custom Vision の Subscription Key と URL をセット
// サブスクリプション画面に表示される URL および Key をコピーしてください
var predictionKey = "YOUR_PREDICTION_KEY";
var endpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/customvision/v3.0/Prediction/YOUR_APP_ID/classify/iterations/YOUR_APP_ITERATION/image";
```

CustomVision.html を開き、画像をアップロードして動作を確認できます。

## Custom Vision (Model Export)

### ONNX & UWP

動作確認環境: Windows 10 (Build 18362.175), Windows SDK 10.0.17763, Visual Studio 2019 (v16.1)

一式をローカルに保存して、Visual Studio で開き、ビルドを行います。(必要なライブラリーが読み込まれます)
デフォルトで ONNX モデルが含まれていますので、[テスト画像](test_images/Dog) をロードして動作を確認できます。

![](doc_images/onnxsample01.png)


#### 自分で Custom Vision から作成したモデルを利用する方法

Custom Vision Portal で **Compact** タイプのプロジェクトを作成します。(既存のプロジェクトを Compact に変更するには、設定画面から "Compact" に変更して再学習させます。)

![](doc_images/customvision21.png)

ONNX (v1.2) でモデルを Export し、ダウロードします。

![](doc_images/customvision22.png)
![](doc_images/customvision23.png)
![](doc_images/customvision24.png)

ダウンロードした ONNX モデルを **CustomVision.onnx** に名前を変更し、[Assets](samples/ONNX/Assets) フォルダーにある CustomVision.onnx と入れ替えます。

再度ビルド＆デバック実行を行い、動作を確認してください。

## Form Recognizer

Form Recognizer の エンドポイント(Endpoint URL)、キー (Subscription Key) および モデル Id (ModelId) の箇所にご自分の情報を入力します。

Azure Portal から

- **エンドポイント** に表示されている文字列が エンドポイント(Endpoint URL) になります。
- **キー1** に表示されている文字列が キー (Subscription Key) になります。

<img src="doc_images/formrecognizer01.png" width="600">

Form OCR Tools から

- **Model ID** に表示されている文字列が モデル Id (ModelId) になります。

<img src="doc_images/formrecognizer02.png" width="600">


****

### C#

[FormRecognition.cshtml.cs](samples/CSharp/CogServicesVisionSamples_202107/Pages/FormRecognition.cshtml.cs)

```FormRecognition.cshtml.cs
// Setting for using FormRecognizer
private const string frKey = "YOUR_FORMRECOGNIZER_KEY";
private const string frEndpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/";
private const string frModelId = "YOUR_FORMRECOGNIZER_MODELID";
```

Visual Studio プロジェクトを開き、NuGet パッケージの復元を行います。ビルド＆起動して、localhost:<YOUR_PORT>/FormRecognition にアクセスし、画像をアップロードして動作を確認できます。

### HTML/JavaScript

[formrecognizer_script.js](samples/JavaScript/scripts/formrecognizer_script.js)

```formrecognizer_script.js
    // Form Recognizer の Subscription Key と URL をセット
    // Azure Portal 画面に表示される URL および Key をコピーしてください
    var subscriptionKey = "YOUR_SUBSCRIPTION_KEY";
    var endpoint = "https://YOUR_LOCATION.api.cognitive.microsoft.com/";
    
    // Form Recognizer の Model Id をセット
    // FOTT ツールに表示される Model Id をコピーしてください
    var modelId = "YOUR_MODEL_ID";
```

[FormRecognizer.html](samples/JavaScript/FormRecognizer.html) を開き、画像をアップロードして動作を確認できます。
