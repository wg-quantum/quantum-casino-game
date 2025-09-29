# Unity 量子コンピュータ学習ゲーム — README

> 本リポジトリは、Unity を用いて量子計算の概念（重ね合わせ・測定・干渉 等）を体験できるゲームのプロジェクトです。以下の手順に従って環境を構築し、実行してください。

---

## 目次

* [概要](#概要)
* [動作環境 / 前提条件](#動作環境--前提条件)
* [環境構築](#環境構築)
* [実行方法](#実行方法)
* [Git 関連（運用ルール）](#git-関連運用ルール)
* [推奨エディタ設定](#推奨エディタ設定)
* [フォルダ構成（目安）](#フォルダ構成目安)
* [トラブルシューティング](#トラブルシューティング)
* [ライセンス / 著作権](#ライセンス--著作権)
* [貢献方法](#貢献方法)

---

## 概要

* 量子ビット（Qubit）やゲート操作を**直感的に操作**して学べる教育／体験型ゲーム。
* 企画・実装・アセットは順次更新予定。**Unity 6.2 beta** を前提にしています（別バージョンで開くと自動アップグレードが走る可能性があるため注意）。

---

## 動作環境 / 前提条件

* **OS**: Windows 10/11, macOS 12+（Apple Silicon/Intel いずれも可）
* **Unity**: **Unity 6.2 beta**（Unity Hub 経由でインストール）
* **Unity Hub**: 最新版
* **Git**: 2.x 系
* **（任意）Git LFS**: 大容量アセットを扱う場合に使用

> **重要**: Unity 6.2 beta 以外のバージョンで開かないでください。別バージョンで開くと `ProjectVersion.txt` が書き換わり、他メンバーの環境に影響します。

---

## 環境構築

1. **Unity のインストール**

   * \[Unity Hub] をインストール
   * Hub を起動し、サインイン

2. **Unity Hub で Unity 6.2 beta をインストール**

   * Hub 左側 **Installs** → **Install Editor**
   * バージョン一覧から **6.2 beta** を選択し、必要な Build Support（Windows/Mac/Android/iOS 等）を追加

3. **リポジトリのクローン**

   ```bash
   git clone <REPO_URL>
   cd <REPO_DIR>
   ```

   * 大容量ファイルを使う場合は Git LFS を有効化:

   ```bash
   git lfs install
   git lfs pull
   ```

4. **クローンしたリポジトリを Unity 6.2 beta で開く**

   * Unity Hub → **Projects** → **Add** → クローンしたフォルダを選択
   * **Unity 6.2 beta** を指定して起動
   * 初回起動時はインポート・コンパイルに時間がかかることがあります

> **注意**: 起動時に「別バージョンで開きますか？」と出たら **キャンセル** し、必ず **6.2 beta** を選び直してください。

---

## 実行方法

1. Unity を起動し、`Assets/Scenes/` 配下からエントリーシーン（例: `Main.unity` または `Title.unity`）を開く
2. エディタ上部の **▶︎ Play** ボタンで実行
3. ビルドする場合: **File → Build Settings**

   * **Scenes In Build** に必要なシーンが含まれていることを確認
   * **Platform** を選択（例: `PC, Mac & Linux Standalone`）→ **Build** または **Build And Run**

> **補足**: TextMeshPro を使用している場合、初回実行で **TMP Essentials** の導入ダイアログが表示されたら **Import** を実行してください。

---

## Git 関連（運用ルール）

1. **master ブランチに更新がある場合、必ず `pull` してからプロジェクトを立ち上げる**

   ```bash
   git checkout master
   git pull origin master
   ```

2. **自身の作業分は `master` からブランチを切って PR を出す**

   * ブランチ命名規則（例）

     * 機能追加: `feature/<概要>`
     * 機能のアップデート: `update/<概要>`
     * 不具合修正: `fix/<概要>`
     * 調整/リファクタ: `refactor/<概要>`

   ```bash
   git checkout -b feature/coin-toss
   # 作業 → コミット
   git push -u origin feature/coin-toss
   ```

   * GitHub/GitLab 上で Pull Request を作成し、レビューを依頼
   * レビュー承認後、`Squash and merge` もしくは `Merge commit`（チーム方針に従う）

3. **コミットメッセージ（推奨）**

   * 先頭に種別: `feat:`, `update:`, `fix:`, `refactor:`, `docs:` など
   * 例: `feat: add coin toss logic using physics` / `update: improve coin spin animation` / `fix: resolve null ref in QuantumGateManager`

4. **競合回避の基本**

   * 作業前/PR 作成前に最新を反映: `git fetch --all` → `git rebase origin/master`（または `merge`）
   * こまめなコミットと小さな PR を心がける

---

## 推奨エディタ設定

* **推奨エディタ**: Visual Studio Code (VS Code)

  * Unity 用拡張機能（C# / Debugger for Unity / Shader support など）をインストール
* **Version Control**

  * `Edit → Project Settings → Editor`

    * **Version Control**: `Visible Meta Files`
    * **Asset Serialization**: `Force Text`
* これにより、**差分がテキスト化**され、**メタファイルの欠落による参照切れ**を防止できます。

---

## フォルダ構成（目安）

```
Assets/
  Scenes/          # シーンファイル（Main.unity / Title.unity など）
  Scripts/         # C# スクリプト
  Art/             # 画像・モデル・マテリアル
  Audio/           # 効果音・BGM
  Prefabs/         # 再利用可能なプレハブ
  Quantum/         # 量子ロジック（ゲート・状態・可視化）
  UI/              # UI 関連（Canvas, TMP 等）
  Settings/        # プロジェクト設定（Input System など）
```

---

## トラブルシューティング

* **別バージョンで開いてしまった**

  * `ProjectSettings/ProjectVersion.txt` が変更されていないか確認。変更されていたら破棄し、**Unity 6.2 beta** で開き直す
* **パッケージ復元に失敗する / コンパイルエラーが多発**

  * **Window → Package Manager** で依存パッケージが解決されているか確認
  * `Library/` を削除してクリーン再インポート（※初回ビルドに時間がかかります）
* **TextMeshPro のフォントが表示されない**

  * `Window → TextMeshPro → Import TMP Essential Resources` を実行
* **Git LFS の大容量ファイルが欠落**

  * `git lfs install && git lfs pull` を実行
* **Play で落ちる／黒画面**

  * `Edit → Project Settings → Player` の `Scripting Backend`/`API Compatibility Level` を確認
  * Console のエラーを参照し、該当スクリプト/アセットを修正

---

## ライセンス / 著作権

* ソースコード: リポジトリの `LICENSE` を参照
* 外部アセット（フォント・モデル・音源 等）: それぞれのライセンスに従う（再配布不可の場合あり）
* スクリーンショットやビルド配布時は、アセットの使用条件を再確認してください。

本資料の著作権は、⽇本アイ・ビー・エム株式会社（IBM Corporationを含み、以下、IBMといいます。）に帰属します。

ワークショップ、セッション、および資料は、IBMまたはセッション発表者によって準備され、それぞれ独⾃の⾒解を反映したものです。 それらは情報提供の⽬的のみで提供されており、いかなる参加者に対しても法律的またはその他の指導や助⾔を意図したものではなく、またそのような結果を⽣むものでもありません。本資料に含まれている情報については、完全性と正確性を期するよう努⼒しましたが、「現状のまま」提供され、明⽰または暗⽰にかかわらずいかなる保証も伴わないものとします。本資料またはその他の資料の使⽤によって、あるいはその他の関連によって、いかなる損害が⽣じた場合も、IBMまたはセッション発表者は責任を負わないものとします。本資料に含まれている内容は、IBMまたはそのサプライヤーやライセンス交付者からいかなる保証または表明を引きだすことを意図したものでも、IBMソフトウェアの使⽤を規定する適⽤ライセンス契約の条項を変更することを意図したものでもなく、またそのような結果を⽣むものでもありません。

本資料でIBM製品、プログラム、またはサービスに⾔及していても、IBMが営業活動を⾏っているすべての国でそれらが使⽤可能であることを暗⽰するものではありません。本資料で⾔及している製品リリース⽇付や製品機能は、市場機会またはその他の要因に基づいてIBM独⾃の決定権をもっていつでも変更できるものとし、いかなる⽅法においても将来の製品または機能が使⽤可能になると確約することを意図したものではありません。本資料に含まれている内容は、参加者が開始する活動によって特定の販売、売上⾼の向上、またはその他の結果が⽣じると述べる、または暗⽰することを意図したものでも、またそのような結果を⽣むものでもありません。 パフォーマンスは、管理された環境において標準的なIBMベンチマークを使⽤した測定と予測に基づいています。ユーザーが経験する実際のスループットやパフォーマンスは、ユーザーのジョブ・ストリームにおけるマルチプログラミングの量、⼊出⼒構成、ストレージ構成、および処理されるワークロードなどの考慮事項を含む、数多くの要因に応じて変化します。したがって、個々のユーザーがここで述べられているものと同様の結果を得られると確約するものではありません。

記述されているすべてのお客様事例は、それらのお客様がどのようにIBM製品を使⽤したか、またそれらのお客様が達成した結果の実例として⽰されたものです。実際の環境コストおよびパフォーマンス特性は、お客様ごとに異なる場合があります。 IBM、IBM ロゴは、 ⽶国やその他の国におけるInternational Business Machines Corporationの商標または登録商標です。他の製品名およびサービス名等は、それぞれIBMまたは各社の商標である場合があります。現時点での IBM の商標リストについては、 ibm.com/trademarkをご覧ください。

問合せ窓口：IBM 沼田 (kifumi@jp.ibm.com)

© Copyright IBM Corp. 2025
---

## 貢献方法

1. Issue を作成して改善提案・不具合報告
2. `master` からブランチを切って実装
3. PR を作成し、説明とスクリーンショット（UI 変更時）を添付

> ドキュメントの改善（本 README を含む）も歓迎します。誤りや不足があれば、`docs:` プレフィックスで PR をお願いします。
