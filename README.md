# ENOVIA MQL Generator

ENOVIA Schema MQL을 빠르고 일관된 형식으로 생성하는 Windows 프로그램입니다.

현재는 **Attribute MQL 생성**을 지원하며, 향후 Type, Relationship 등 다른 Schema 생성 기능을 추가할 예정입니다.

## 주요 기능

- 입력값을 반영한 MQL 실시간 미리보기
- 여러 Range 값 추가 및 삭제
- `multiline`, `hidden` 옵션 지원
- 생성된 MQL 클립보드 복사
- Schema 폴더에 바로 저장
- 원하는 위치에 MQL 파일 저장
- Application, Version, Installer 등 공통값 설정

## 실행 방법

Windows에서 다음 파일을 실행합니다.

```text
Release File\MQLGenerator.exe
```

별도의 설치 과정은 필요하지 않습니다.

프로그램 설정은 EXE와 같은 폴더의 `MQLGenerator.ini`에 저장되며, 실행 로그는 `MQLGenerator.log`에 기록됩니다.

## Attribute 사용법

Attribute 입력 항목, Range 처리, 설정값과 생성 결과에 대한 자세한 설명은 다음 문서를 참고하세요.

[Attribute 사용 가이드](docs/Attribute.md)

## 기본 저장 위치

`Save to schema`를 누르면 기본적으로 EXE 위치를 기준으로 다음 폴더에 저장합니다.

```text
..\..\01.Attribute
```

현재 프로젝트 구조에서는 다음 위치에 `{AttributeName}.mql` 파일이 생성됩니다.

```text
schema\01.Attribute
```

저장 위치는 `Tools > Settings...`에서 변경할 수 있습니다.

## 프로젝트 구조

```text
.
├─ assets
│  └─ MQLGenerator.ico
├─ docs
│  └─ Attribute.md
├─ Release File
│  ├─ MQLGenerator.exe
│  └─ MQLGenerator.ini
└─ src
   └─ MQLGenerator
      ├─ Program.cs
      ├─ MainForm.cs
      ├─ Generators
      │  └─ AttributeGenerator.cs
      ├─ Settings
      │  ├─ AppSettings.cs
      │  └─ SettingsForm.cs
      ├─ Styles
      │  └─ UiStyle.cs
      └─ Views
         └─ AttributeView.cs
```

## 소스 빌드

.NET Framework C# 컴파일러로 전체 소스를 함께 컴파일합니다.

```powershell
$sources = Get-ChildItem .\src\MQLGenerator -Recurse -Filter *.cs

& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" `
  /nologo `
  /codepage:65001 `
  /target:winexe `
  /win32icon:assets\MQLGenerator.ico `
  /out:"Release File\MQLGenerator.exe" `
  /reference:System.dll `
  /reference:System.Drawing.dll `
  /reference:System.Windows.Forms.dll `
  $sources.FullName
```

## 주의사항

- 실제 운영 환경에 적용하기 전에 생성된 MQL을 반드시 검토하세요.
- 회사명, 사용자명, 서버 정보 등 민감한 데이터는 저장소에 올리지 마세요.
- 로그 파일(`*.log`)은 Git에서 제외됩니다.
