# Attribute MQL Template

`schema/01.Attribute`에 새 attribute MQL을 빠르게 만들기 위한 템플릿입니다.

## GUI 사용

PowerShell 없이 브라우저에서 `schema\00.Template\01.Attribute\attribute-ui.html` 파일을 열어 사용합니다.

입력 후 `Download`를 누르면 `{Name}.mql` 파일로 저장할 수 있습니다. `Save As`는 브라우저가 파일 저장 API를 지원하면 직접 저장 창을 열고, 지원하지 않으면 다운로드로 동작합니다.

## EXE 사용

`schema\00.Template\01.Attribute\AttributeMqlGenerator.exe`를 실행합니다.

`Save to schema`를 누르면 `schema\01.Attribute\{Name}.mql`에 저장합니다. `Save As`를 누르면 원하는 위치를 직접 선택할 수 있습니다.

상단 메뉴의 `Tools > Settings...`에서 `Application`, `Version`, `Installer`, `Create by`, 저장 폴더 기본값을 수정할 수 있습니다. 설정은 exe와 같은 폴더의 `AttributeMqlGenerator.ini`에 저장됩니다.

## 바로 생성

PowerShell 실행 정책 때문에 막히면 현재 창에서 한 번만 실행합니다.

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

```powershell
.\schema\00.Template\01.Attribute\new-attribute.ps1 `
  -Name custSampleAttribute `
  -DescriptionKo "샘플 속성" `
  -DescriptionEn "Sample Attribute"
```

`Create by` 값은 기본적으로 `CREATED_BY` 환경변수를 먼저 사용하고, 값이 없으면 Windows `USERNAME` 환경변수를 사용합니다.

현재 PowerShell 창에서만 작성자 이름을 지정하려면 다음처럼 실행합니다.

```powershell
$env:CREATED_BY = "HongGilDong"
```

특정 파일을 만들 때만 작성자 이름을 직접 지정할 수도 있습니다.

```powershell
.\schema\00.Template\01.Attribute\new-attribute.ps1 `
  -Name custSampleAttribute `
  -DescriptionKo "샘플 속성" `
  -DescriptionEn "Sample Attribute" `
  -CreatedBy "HongGilDong"
```

## Range가 있는 속성

여러 range 값은 쉼표로 구분해서 넘깁니다. PowerShell 배열 문법이라 `A,B,C`가 각각 하나의 값으로 전달됩니다.

```powershell
.\schema\00.Template\01.Attribute\new-attribute.ps1 `
  -Name custSampleFlag `
  -DescriptionKo "샘플 구분" `
  -DescriptionEn "Sample Flag" `
  -Range A,B,C
```

생성 결과:

```mql
    range = 'A'
    range = 'B'
    range = 'C'
```

`A,B` 자체가 하나의 range 값이면 따옴표로 감싸서 넘깁니다.

```powershell
.\schema\00.Template\01.Attribute\new-attribute.ps1 `
  -Name custSampleCombinedFlag `
  -DescriptionKo "샘플 복합 구분" `
  -DescriptionEn "Sample Combined Flag" `
  -Range "A,B",C
```

생성 결과:

```mql
    range = 'A,B'
    range = 'C'
```

## Date 속성

```powershell
.\schema\00.Template\01.Attribute\new-attribute.ps1 `
  -Name custSampleDate `
  -DescriptionKo "샘플 일자" `
  -DescriptionEn "Sample Date" `
  -Type date
```

생성 위치는 `schema/01.Attribute/{Name}.mql`입니다. 기본 property 값은 `Framework`, `R2026x`, `CUST`로 설정되어 있습니다.
