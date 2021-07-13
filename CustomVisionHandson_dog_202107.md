# Microsoft Cognitive Services を利用した 画像識別アプリ 開発 (202107 版: 犬種判定(Dogs Classification))

"人工知能 API" [Microsoft Azure Cognitive Services](https://www.microsoft.com/cognitive-services/) や [Microsoft Azure Applied AI Services](https://azure.microsoft.com/ja-jp/product-categories/applied-ai-services/) を使うと、画像分析を行うエンジンをノーコーディングで利用、作成できます。

[Custom Vision Service](https://azure.microsoft.com/ja-jp/services/cognitive-services/custom-vision-service/) は、ご自分で用意した画像をアップロードしてタグ付け、学習させることで、画像の分類 (Classification) や 画像に写っているモノの抽出 (Object Detection) を行うエンジンを簡単に作成でき、Web API として利用できます。また TensorFlow / CoreML / ONNX、または Docker コンテナー向けに Export して利用することもできます。

ここでは、Custom Vision で画像識別エンジンを作成し、Web アプリ (C# または HTML&JavaScript) からアクセスして利用する方法を紹介します。

> アプリの動作は、[オンラインアプリサンプル](http://cogservicesvisionsamples201906.azurewebsites.net/CustomVisionClassification-Dog) でご確認ください。


## 目次

0. [準備](#準備)
1. [Custom Vision で画像識別エンジンを作成](#1-custom-vision-で画像識別エンジンを作成)
2. Web アプリから Custom Vision にアクセス
    - [HTML&JavaScript 版](#htmljavascript-版)
    - [C# 版](#c-版)


## 準備

- Azure サブスクリプション & Custom Vision のサービス申込
    - Azure サブスクリプション の申し込みには マイクロソフトアカウントが必要です。
    - [Azure 無償サブスクリプション準備](https://qiita.com/annie/items/3c9ddc3fb8f120769239) の手順で、Azure サブスクリプション申込を行います(無償以外の有償アカウント等でも問題ありません)。
    - [Cognitive Services サブスクリプション準備]() の手順で、Custom Vision のサービス作成を行います。

- [CogServicesVisionSamples](https://github.com/ayako/CogServicesVisionSamples) のディレクトリをローカルにダウンロードしておきます。**[Clone or download]** をクリックして、ZIP でダウンロードできます。

<img src="doc_images/CogServicesVisinSamples_download.png" width="600">



## 手順

### 1. Custom Vision で画像識別エンジンを作成

ブラウザーから [Custom Vision のポータル (https://www.customvision.ai)](https://www.customvision.ai) にアクセスします。
**[SIGN IN]** をクリックし、Azure & Custom Vision の サブスクリプションが紐づいたアカウントでサインインします。

<img src="doc_images/handson_customvision_31.png" width="600">

**[NEW PROJECT]** をクリックし、画像識別エンジンを新規に作成するダイアログを開きます。

<img src="doc_images/handson_customvision_32.png" width="600">

**Create New Project** ダイアログで、**Name** にお好きな名前を入力します。Project Type は **Classification** を選択し、Classification は **Multiclass**　を選択します。**[Create Project]** をクリックして、新規プロジェクトを作成します。

<img src="doc_images/handson_customvision_33.png" width="600">

プロジェクトが作成されると詳細画面が表示されます。 **Add Images**　をクリックして、画像識別エンジンに学習させる画像をアップロードするダイアログを開きます。

<img src="doc_images/handson_customvision_34.png" width="600">

ダウンロードしたコンテンツの [training_images\Dog](training_images/Dog) フォルダーにある画像を選択します。まずは [beagle](training_images\Dog\beagle) フォルダーにある画像をすべて選択して **[開く]** をクリックします。

<img src="doc_images/handson_customvision_35.png" width="600">

**My Tags** に **ビーグル** と入力し、**[Upload Files]** をクリックします。

<img src="doc_images/handson_customvision_36.png" width="600">

これで *ビーグル* のタグが付いた画像がアップロードできました。**[Done]** をクリックしてダイアログを閉じます。

<img src="doc_images/handson_customvision_37.png" width="600">

同様に、他の画像もタグをつけてアップロードします。

| 画像フォルダー名 | タグ (犬の品種名) |
|:------------|:---------------------|
| beagle | ビーグル |
| bernese-mountain-dog | バーニーズマウンテンドッグ |
| chihuahua | チワワ |
| eskimo-dog | エスキモードッグ |
| german-shepherd | ジャーマンシェパード |
| golden-retriever | ゴールデンレトリバー |
| maltese-dog | マルチーズ |

画像のアップロードが終了したら、上部バーの **Train** をクリックして、画像をエンジンに学習させます。

<img src="doc_images/handson_customvision_38.png" width="600">

**Training Type** は **Quick Training** を選択し、**Train** をクリックして学習させます。

<img src="doc_images/handson_customvision_39.png" width="600">

学習が終了すると、**Performance** のページに学習結果が表示されます。このエンジンをテストするには、上部バーから **Quick Test** をクリックします。

<img src="doc_images/handson_customvision_40.png" width="600">

**Browse local files** をクリックして、ダウンロードしたコンテンツの [test_images\Dog](test_images/Dog) フォルダーにある画像をひとつ選択します。画像の犬種が正しく識別されたら OK です。

<img src="doc_images/handson_customvision_41.png" width="600">

> 必要に応じて、画像の追加→学習→テスト を繰り返します

**Performance** の画面で、上部バーから **Publish** をクリックして、Web API から利用できるように発行を行います。

<img src="doc_images/handson_customvision_42.png" width="600">

**Publish Name** のダイアログで、以下のように設定します;

- Model Name: Iteration1 (デフォルト)
- Prediction resource: Azure Portal で作成した Custom Vision Prediction を選択します

 **Publish** をクリックして発行します。

<img src="doc_images/handson_customvision_43.png" width="600">

Publish が完了すると **Prediction URL** が表示されるので、クリックして作成した画像判別エンジンにアクセスする URL の情報を表示します。

<img src="doc_images/handson_customvision_44.png" width="600">

*If you have an image fIle* の下に表示されている URL と *Prediction-Key* の横に表示されている Prediction Key をコピーして、ローカルに保存しておきます。

<img src="doc_images/handson_customvision_45.png" width="600">


### 2. Web アプリ (HTML&JavaScript) から Custom Vision にアクセス

#### HTML&JavaScript 版

ダウンロードしておいたコンテンツの [samples\JavaScript](samples/JavaScript) フォルダーにあるソースを編集します。

[scripts\customvision_script.js](samples/JavaScript/scripts/customvision_script.js) をコードエディターで開きます。

18,19　行目の predictionKey と endpoint の設定箇所をローカルに保存しておいた URL と Prediction Key で書き換えます。

```
    var predictionKey = "ローカルに保存しておいたPrediction Key";
    var endpoint = "ローカルに保存しておいたURL";
```

customvision_script.js を保存し、ブラウザーで開きます。

<img src="doc_images/handson_customvision_16.png" width="600">

**[ファイルを選択]** をクリックして、[test_images\Dog](test_images/Dog) フォルダーにある画像をひとつ選択します。画像が識別されて犬種情報が表示されたら完了です。

<img src="doc_images/handson_customvision_17d.png" width="600">


#### C# 版

ダウンロードしておいたコンテンツの [samples\CSharp](samples/CSharp) フォルダーにあるソースを編集します。

[CogServicesVisionSamples_202107.sln](samples/CSharp/CogServicesVisionSamples_202107/CogServicesVisionSamples_202107.sln) を Visual Studio で開きます。

[Pages\CustomVisionClassification.cshtml.cs](samples/CSharp/CogServicesVisionSamples_202107/Pages/CustomVisionClassification.cshtml.cs) をクリックして開きます。

<img src="doc_images/handson_customvision_18.png" width="600">

22~25　行目の predictionKey と endpoint の設定箇所をローカルに保存しておいた URL と Prediction Key で書き換えます。

```
    private const string cvPredictionKey = "YOUR_CUSTOMVISION_PREDICTION_KEY";
    private const string cvEndpoint = "https://YOUR_SERVICE_NAME.cognitiveservices.azure.com/";
    private const string cvProjectId = "YOUR_CUSTOMVISION_PROJECTID";
    private const string cvPublishName = "YOUR_CUSTOMVISION_PROJECT_PUBLISHNAME";//"Iteration1"
```

保存しておいた URL が以下のようになっている場合、

<pre>https://customvision01.cognitiveservices.azure.com/customvision/v3.0/Prediction/a0000000-0000-0000-aaaa-000000000000/classify/iterations/Iteration1/image </pre>

 - YOUR_CUSTOMVISION_PREDICTION_KEY -> 保存しておいた predicitionKey
 - YOUR_SERVICE_NAME -> customvision01
 - YOUR_CUSTOMVISION_PROJECTID -> a0000000-0000-0000-aaaa-000000000000
 - YOUR_CUSTOMVISION_PROJECT_PUBLISHNAME -> Iteration1

となります。

上部バーの ▶ をクリックして、ビルド＆アプリの起動を行います。

<img src="doc_images/handson_customvision_19.png" width="600">

ブラウザーが起動して、Web アプリの画面が表示されます。上部バーから **Custom Vision Classification** をクリックします。

<img src="doc_images/handson_customvision_20.png" width="600">

*Custom Vision Classification* の画面で、**[ファイルを選択]** をクリックして、[test_images\Dog](test_images/Dog) フォルダーにある画像をひとつ選択します。**[Analyze]** をクリックすると画像が識別されて犬種情報が表示されたら完了です。

<img src="doc_images/handson_customvision_21d.png" width="600">
