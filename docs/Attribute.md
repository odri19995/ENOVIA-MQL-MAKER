# Attribute MQL 사용 가이드

MQL Generator의 Attribute 화면은 입력값을 바탕으로 ENOVIA Attribute 생성 MQL을 만듭니다.

## 입력 항목

| 항목 | 설명 |
|---|---|
| Name | 생성할 Attribute 이름 |
| Description KO | 주석에 표시할 한글 설명 |
| Description EN | Attribute의 `description` 값 |
| Type | `string`, `date`, `integer`, `real`, `boolean` 중 선택 |
| Default | Attribute 기본값 |
| Created Date | 파일 상단 주석에 표시할 생성일 |
| Installed Date | `installed date` property 값 |
| Create by | 파일 상단 주석에 표시할 작성자 |
| Application | `application` property 값 |
| Version | `version` property 값 |
| Installer | `installer` property 값 |

## 옵션

### multiline

선택하면 다음 구문이 생성됩니다.

```mql
multiline
```

선택하지 않으면 `notmultiline`이 생성됩니다.

### hidden

선택하면 다음 구문이 생성됩니다.

```mql
hidden
```

선택하지 않으면 `nothidden`이 생성됩니다.

## Range 사용법

Range 입력창에 값을 입력하고 `Add`를 누르면 Range가 한 개 추가됩니다. Enter 키로도 추가할 수 있습니다.

```text
A
B
C
```

위 값을 각각 추가하면 다음 MQL이 생성됩니다.

```mql
range = 'A'
range = 'B'
range = 'C'
```

쉼표는 자동으로 분리하지 않습니다. 따라서 `A,B`를 한 번에 추가하면 하나의 Range 값으로 처리됩니다.

```mql
range = 'A,B'
```

- `Remove`: 선택한 Range 삭제
- `Clear`: 모든 Range 삭제

## 저장 기능

### Save to schema

설정된 Schema 폴더에 다음 파일명으로 저장합니다.

```text
{Name}.mql
```

기본 저장 경로는 EXE 위치 기준 `..\..\01.Attribute`입니다. 대상 폴더가 없으면 자동으로 생성합니다.

같은 파일이 이미 있으면 덮어쓸 것인지 확인합니다.

### Save As

Windows 파일 저장 창을 열어 사용자가 직접 위치와 파일명을 선택합니다.

### Copy

미리보기의 전체 MQL을 클립보드에 복사합니다.

## 설정

상단 메뉴의 `Tools > Settings...`에서 공통값을 변경할 수 있습니다.

```ini
Application=Framework
Version=R2026x
Installer=CUST
CreatedBy=USER
SaveFolder=..\..\01.Attribute
```

설정은 EXE와 같은 폴더의 `MQLGenerator.ini`에 저장됩니다.

| 설정 | 설명 |
|---|---|
| Application | 기본 application 값 |
| Version | 기본 version 값 |
| Installer | 기본 installer 값 |
| CreatedBy | 기본 작성자 |
| SaveFolder | `Save to schema` 대상 폴더 |

`SaveFolder`에 상대 경로를 입력하면 EXE가 있는 폴더를 기준으로 계산합니다. 절대 경로도 사용할 수 있습니다.

## 생성 예시

다음과 같이 입력했다고 가정합니다.

```text
Name: custSampleAttribute
Description KO: 샘플 속성
Description EN: Sample Attribute
Type: string
Range: A
Range: B
```

주요 생성 결과는 다음과 같습니다.

```mql
#del attribute 'custSampleAttribute';
add attribute 'custSampleAttribute'
    description 'Sample Attribute' #샘플 속성
    type 'string'
    default ''
    range = 'A'
    range = 'B'
    notmultiline
    nothidden
    property    'application'    value 'Framework'
    property    'version'        value 'R2026x'
    property    'installer'      value 'CUST'
    property    'installed date' value '2026-06-18'
    property    'original name'  value 'custSampleAttribute'
;
add property 'attribute_custSampleAttribute' on program eServiceSchemaVariableMapping.tcl to attribute 'custSampleAttribute';
```

작은따옴표가 포함된 입력값은 MQL 문자열에서 사용할 수 있도록 작은따옴표 두 개로 변환됩니다.
