# Microsoft Azure Applied AI Services を利用した フォーム読み取りアプリ 開発 (202107 版)

"人工知能 API" [Microsoft Azure Cognitive Services](https://www.microsoft.com/cognitive-services/) や [Microsoft Azure Applied AI Services](https://azure.microsoft.com/ja-jp/product-categories/applied-ai-services/) を使うと、画像分析を行うエンジンをノーコーディングで利用、作成できます。

[Form Recognizer](https://azure.microsoft.com/ja-jp/services/form-recognizer/) は帳票、IDなどの定型フォーム画像を読み取るエンジンを作成、すぐに Web API で利用できるサービスです。

ここでは、Form Recognizer を Web アプリ (C# または HTML&JavaScript) からアクセスして、Id カード画像から情報を読み取る方法を紹介します。

> アプリの動作は、オンラインアプリサンプル ([運転免許証読み取り](https://cogservicesvisionsamples202103.azurewebsites.net/FormRecognition)) でご確認ください。

## 目次

0. [準備](#準備)
1. [Form Recognizer を利用する](#Form-Recognizer-を利用する)
    - 1.1. [プリセットモデルをテストする](#プリセットモデルをテストする)
    - 1.2. [カスタムモデルを作成してテストする](#カスタムモデルを作成してテストする)


## 準備

- Azure サブスクリプション & Face API のサービス申込
    - Azure サブスクリプション の申し込みには マイクロソフトアカウントが必要です。
    - [Azure 無償サブスクリプション準備](https://qiita.com/annie/items/3c9ddc3fb8f120769239) の手順で、Azure サブスクリプション申込を行います(無償以外の有償アカウント等でも問題ありません)。
    - [Cognitive Services サブスクリプション準備]() の手順で、Form Recognizer および Azure Storage のサービス作成を行います。
      - Form Recognizer のエンドポイント(Rest API アクセス URL)と、アクセスキー をローカルに保存しておきます。
      - Azure Storage に BLOB コンテナーを作成し、SAS URL をローカルに保存しておきます。 

<img src="doc_images/handson_formrecognizer_00.png" width="600">
<img src="doc_images/handson_storage_00.png" width="600">

- [CogServicesVisionSamples](https://github.com/ayako/CogServicesVisionSamples) のディレクトリをローカルにダウンロードしておきます。**[Clone or download]** をクリックして、ZIP でダウンロードできます。

<img src="doc_images/CogServicesVisinSamples_download.png" width="600">


## 1. Form Recognizer を利用する

Form Recognizer には、カスタムモデル作成やテストを行う GUI ツールである [Form OCR Test Tool](https://fott-2-1.azurewebsites.net/) が用意されています。これを用いてプリセットモデルのテスト、およびカスタムモデルの作成とテストを行います。

### 1.1. プリセットモデルをテストする

[Form OCR Test Tool (https://fott-2-1.azurewebsites.net/)](https://fott-2-1.azurewebsites.net/) をブラウザーで開きます。**Use prebuilt model** をクリックします。

<img src="doc_images/handson_formrecognizer_01.png" width="600">

プリセットモデルのテスト画面が表示されます。右列に分析したい画像や Form Recognizer のサービス情報を設定します。

先に *2.Get Prediction* の欄にForm Recognizer のサービス情報を以下の通り入力します。

- **Form recognizer service endpoint**: ローカルに保存しておいた Form Recognizer のエンドポイント
- **API Key**: ローカルに保存しておいた Form Recognizer のサービスキー
- **Form Type**: 今回は **Id** を選択

<img src="doc_images/handson_formrecognizer_02.png" width="600">

*1.Choose file for analysis* の欄で分析する画像を選択します。 ローカルにダウンロードしておいたディレクトリーから [test_images\Form\US_driverslicense_test.jpg](test_images/Form/US_driverslicense_test.jpg) を選択すると、画面に表示されます。**[Run Analysis]** をクリックします。

<img src="doc_images/handson_formrecognizer_03.png" width="600">

画像をプリセットモデルで分析した読み取り結果が表示されることを確認してください。

<img src="doc_images/handson_formrecognizer_04.png" width="600">

### 1.2. カスタムモデルを作成してテストする

#### 学習データのアップロード

Azure Storage (BLOB コンテナー) に学習データをアップロードします。
[Azure Portal](https://portal.azure.com) をブラウザーで開き、予め作成しておいた Azure Storage を開きます。左列メニューバーから **Storage Explorer** をクリックします。

<img src="doc_images/handson_formrecognizer_05.png" width="600">

作成済みの BLOB コンテナー (ここでは *form-training*) を選択します。**アップロード** をクリックします。

<img src="doc_images/handson_formrecognizer_06.png" width="600">

画面右側に表示される *BLOB のアップロード* ペインの **ファイルの選択** をクリックして、ローカルに保存したディレクトリーから [training_images\Form](training_images/Form) 配下の画像ファイルを全て選択し、アップロードします。

<img src="doc_images/handson_formrecognizer_07.png" width="600">

<img src="doc_images/handson_formrecognizer_08.png" width="600">


#### Form OCR Test Tool を用いたカスタムモデルの作成

[Form OCR Test Tool (https://fott-2-1.azurewebsites.net/)](https://fott-2-1.azurewebsites.net/) をブラウザーで開きます。左端のメニューから **Connections** (コンセントのマーク) をクリックします。

<img src="doc_images/handson_formrecognizer_09.png" width="600">

*Connections* バーにある **＋** をクリックして、Blob コンテナーへの接続を追加しします。

<img src="doc_images/handson_formrecognizer_10.png" width="600">

以下の項目を入力し、**[Save Connection]** をクリックして保存します。

- **Display name**: ご自身で判別しやすい名前を入力
- **Provider**: Azure blob container (デフォルト)
- **SAS URL**: ローカルに保存しておいた BLOB コンテナーの SAS URL

<img src="doc_images/handson_formrecognizer_11.png" width="600">

Connection が保存されたら、左端のメニューから **Home** (家のマーク) をクリックしてトップページに戻ります。

<img src="doc_images/handson_formrecognizer_12.png" width="600">

Form OCR Test Tool のトップ画面で、今度は **Use Custom ...** をクリックします。

<img src="doc_images/handson_formrecognizer_13.png" width="600">

**New Project** をクリックして、新規にカスタムモデルを作成します。

<img src="doc_images/handson_formrecognizer_14.png" width="600">

カスタムモデルの作成に必要な以下の情報を選択 & 入力し、**[Save Project]** をクリックして保存します。

- **Display name**: ご自身で判別しやすい名前を入力
- **Security token**: Generate New Security Token (デフォルト)
- **Source connection**: 前の手順で作成した connection
- **Form Recognizer Service URI**: ローカルに保存しておいた Form Recognizer のエンドポイント
- **API key**: ローカルに保存しておいた Form Recognizer のサービスキー

<img src="doc_images/handson_formrecognizer_15.png" width="600">


Tag Editor の画面に遷移します。Blob コンテナーにアップロードしておいた学習データが読み込まれます。画面右側の **Tags** ペインから検出したい情報のタグ付けを行います。

<img src="doc_images/handson_formrecognizer_16.png" width="600">

*add new tag* に **name** と入力します。

<img src="doc_images/handson_formrecognizer_17.png" width="600">

中央に表示されている画像から名前の文字をクリックして選択し (選択済みは緑色で表示、再度クリックすると選択解除)、タグ一覧の **name** をクリックします。

<img src="doc_images/handson_formrecognizer_18.png" width="600">

画像上で選択した文字がタグの下に表示されていれば OK です。

<img src="doc_images/handson_formrecognizer_19.png" width="600">

同様に、*add new tag* に **birthday** と入力してタグを追加します。画像から生年月日の文字を選択して、**birtyday** をクリック、情報を紐づけます。

<img src="doc_images/handson_formrecognizer_20.png" width="600">

**address** と **expiration** というタグも追加し、住所や有効期限の文字情報を紐付けます。

<img src="doc_images/handson_formrecognizer_21.png" width="600">


他の画像も同じ手順で情報をタグ付けします。1枚目の画像のときに作成したタグが表示されるので、該当する文字情報を選択して紐付けます。

<img src="doc_images/handson_formrecognizer_22.png" width="600">

> カスタムモデルを作成うするためには 5 枚以上の同一のフォーム画像が必要です。


画像のタグ付けが終了したら、データを学習させます。左端のメニューから **Train** (上から 3 つ目) をクリックして開きます。右側に表示される *Train a new model* ペインで、Model name に識別しやすいお好きな名前 (ここでは **Iteration1**) を入力し、**[Train]** をクリックして学習させます。

<img src="doc_images/handson_formrecognizer_23.png" width="600">

しばらくして *Train Result* が表示されたら学習は完了です。

<img src="doc_images/handson_formrecognizer_24.png" width="600">

左端のメニューから **Analyze** (上から 5 つ目) をクリックして開きます。
右側のペインで *1.Choose an image for analyze with* の欄で分析する画像を選択します。 ローカルにダウンロードしておいたディレクトリーから [test_images\Form\JP_driverslicense_test.jpg](test_images/Form/JP_driverslicense_test.jpg) を選択します。

<img src="doc_images/handson_formrecognizer_25.png" width="600">

**[Run Analysis]** をクリックすると、学習させたカスタムモデルで分析した読み取り結果が表示されます。

<img src="doc_images/handson_formrecognizer_26.png" width="600">
