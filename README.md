WindowController
===


## Description
Windowsアプリケーションのウィンドウを操作するためのライブラリです。  
.batでexeを起動する時の引数を拡張して、ウィンドウの操作を設定します。

## Usage
WindowController\\Prefabs\\Window Controller.prefabをゲーム空間に置いてください。

.batでexe起動を設定する時に、必要に応じて引数を追記してください。  
現在対応している引数は以下の通りです。
- --pos-x 100 : ウィンドウの左上X座標を100に指定します。
- --pos-y 100 : ウィンドウの左上Y座標を100に指定します。
- --topmost 1 : ウィンドウを最前面に設定します。

#### batの例
// X座標が100、Y座標が100、最前面表示で縁無しウィンドウとして実行  
.\\WindowController.exe -popupwindow --pos-x 100 --pos-y 100 --topmost 1

より詳しい使い方は[Example](Assets/WindowController/Example/)を参照してください。

## Behaviour
- 縁無しウィンドウでないと機能しないので、必ず-popupwindowを指定してください。
- win32APIを利用してウィンドウを操作・設定しています。
- 最前面設定を行うと、デフォルトでは一定間隔毎にトップ化処理をします。

## API
### `WindowController`クラス
`WindowController`からウィンドウ操作に関する設定を変更する事が出来ます。  
シングルトンを実装してありますので、`WindowController.Instance`でアクセス出来ます。

### プロパティ
#### `bool IsValidity`
動作可能な環境かどうかを取得

#### `bool LoopMostValidity`
一定間隔毎のトップ化処理の有効化

#### `bool MostType`
トップ化処理の種類を設定  
最前面か非最前面の2種類が存在します
