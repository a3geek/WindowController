# WindowController
Windowsアプリケーションのウィンドウを操作するためのライブラリ。

## 使い方
WindowSetting.csを適当なゲームオブジェクトにアタッチする。  
アプリケーションを起動する時の.batにオプションを追加する。
- --pos-x ウィンドウのX座標を10に指定する
- --pos-y ウィンドウのY座標を10に指定する
- --topmost ウィンドウを最前面表示にさせる

## .batの例
// X座標が10、Y座標が20で最前面表示で実行する  
C:\teamLab\NurieTown3D.exe --pos-x 10 --pos-y 20 --topmost 1

## リリース
http://github.team-lab.local/SketchSeries/WindowController/releases

## その他
WinAPIを利用しているのでWindowsのみ対応
