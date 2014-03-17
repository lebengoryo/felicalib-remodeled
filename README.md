## felicalib 改

felicalib 改 (felicalib Remodeled) is a FeliCa access wrapper library for .NET, forked from [tmurakam/felicalib](https://github.com/tmurakam/felicalib).

### Setup
felicalib 改は、[NuGet Gallery](http://www.nuget.org/packages/FelicaLib.DotNet/) に登録されています。  
felicalib 改を利用するには、次のいずれかの方法で NuGet パッケージをインストールします。

* Visual Studio の [パッケージ マネージャー コンソール] で、次のコマンドを実行します。

```
Install-Package FelicaLib.DotNet
```

* Visual Studio でプロジェクトを右クリックして、[Nuget パッケージの管理] で「felicalib Remodeled」を検索してインストールします。「felica」と入力すれば見つかります。

![VS-NuGet](Images/Preview/VS-NuGet.png)

### Release notes
* **v1.1.0** NuGet に登録しました (この時点でソースコードは変更していません)。
* **v1.1.6** アンマネージ リソースの扱いを改善し、安定性を向上させました。
* **v1.2.26** クラスを再設計し、安定性を向上させました。
* **v1.2.48** ユーティリティ メソッドを追加するなど、API を改善しました。

### Future plans
* **v1.3** 一定間隔の自動ポーリング。
* **v1.4** WebSocket サーバーによるホスト。
