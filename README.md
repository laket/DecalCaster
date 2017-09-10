# 概要
Deffered Decalで動的なテクスチャを投射するサンプルコード。[Unityのマニュアルコード](https://docs.unity3d.com/Manual/GraphicsCommandBuffers.html
)がベース。

クリックした点を中心にランダムな数列を投射する。

# 使い方
System.DrawingをUnity\Editor\Data\Mono\lib\mono\2.0から持ってこないとコンパイルできない。

# 実装メモ
Main CameraのRendering PathをDeferredにするのを忘れないこと。

