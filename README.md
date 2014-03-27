## felicalib 改

felicalib 改 (felicalib Remodeled) is a FeliCa access wrapper library for .NET, forked from [tmurakam/felicalib](https://github.com/tmurakam/felicalib).

### Setup
felicalib 改は、[NuGet Gallery](http://www.nuget.org/packages/FelicaLib.DotNet/) に登録されています。  
felicalib 改を利用するには、次の方法で NuGet パッケージをインストールします。

**方法 1.** Visual Studio の [パッケージ マネージャー コンソール] で、次のコマンドを実行します。

```
Install-Package FelicaLib.DotNet
```

**方法 2.** Visual Studio でプロジェクトを右クリックして、[Nuget パッケージの管理] で「felicalib Remodeled」を検索してインストールします。「felica」と入力すれば見つかります。

![VS-NuGet](Images/Preview/VS-NuGet.png)

### Usage
まず、FelicaLib 名前空間を追加します。

```c#
using FelicaLib;
```

Edy の残高を取得するなどの一部の特殊な用途に対しては、ヘルパー メソッドが用意されています。

```c#
int balance = FelicaHelper.GetEdyBalance();
```

システム コード、サービス コードおよびアドレスを指定して、非暗号化領域のデータを取得します。

```c#
byte[] data = FelicaUtility.ReadWithoutEncryption(FelicaSystemCode.Edy, 0x1317, 0);
int balance = data.ToEdyBalance();
```

### Testing environment
* [PaSoRi RC-S380](http://www.sony.co.jp/Products/felica/consumer/products/RC-S380.html)
* Windows 8
* .NET Framework 3.5, 4.5

### Release notes
* **v1.1.0** NuGet に登録しました (この時点でソースコードは変更していません)。
* **v1.1.6** アンマネージ リソースの扱いを改善し、安定性を向上させました。
* **v1.2.26** クラスを再設計し、安定性を向上させました。
* **v1.2.48** ユーティリティ メソッドを追加するなど、API を改善しました。
* **v1.2.67** 複数のブロックを連続的に読み込むためのメソッドを追加しました。

### Future plans
* **v1.3** 一定間隔の自動ポーリング。
* **v1.4** WebSocket サーバーによるホスト。
