[CmdletBinding(PositionalBinding = $false)]
param(
    [Parameter(Mandatory = $true)]
    [string]$Name,

    [Parameter(Mandatory = $true)]
    [string]$DescriptionKo,

    [Parameter(Mandatory = $true)]
    [string]$DescriptionEn,

    [ValidateSet('string', 'date', 'integer', 'real', 'boolean')]
    [string]$Type = 'string',

    [string]$Default = '',

    [string]$CreatedBy = $(if ($env:CREATED_BY) { $env:CREATED_BY } else { 'USER' }),

    [string]$Application = 'Framework',

    [string]$Version = 'R2026x',

    [string]$Installer = 'CUST',

    [switch]$Multiline,

    [switch]$Hidden,

    [switch]$Force,

    [string[]]$Range = @()
)

$ErrorActionPreference = 'Stop'

$templatePath = Join-Path $PSScriptRoot 'attribute.mql.tpl'
$schemaRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..\01.Attribute')
$targetPath = Join-Path $schemaRoot "$Name.mql"

if ((Test-Path -LiteralPath $targetPath) -and -not $Force) {
    throw "Target already exists: $targetPath. Use -Force to overwrite."
}

$today = Get-Date -Format 'yyyy.MM.dd'
$installedDate = Get-Date -Format 'yyyy-MM-dd'

$rangeText = ''
foreach ($value in $Range) {
    $escaped = $value.Replace("'", "''")
    $rangeText += "    range = '$escaped'`r`n"
}

$descriptionComment = ''
if ($DescriptionKo -and ($DescriptionKo -ne $DescriptionEn)) {
    $descriptionComment = " #$DescriptionKo"
}

$tokens = @{
    '{{CREATED_DATE}}'        = $today
    '{{CREATED_BY}}'          = $CreatedBy
    '{{DESCRIPTION_KO}}'      = $DescriptionKo
    '{{ATTRIBUTE_NAME}}'      = $Name
    '{{DESCRIPTION_EN}}'      = $DescriptionEn
    '{{DESCRIPTION_COMMENT}}' = $descriptionComment
    '{{ATTRIBUTE_TYPE}}'      = $Type
    '{{DEFAULT_VALUE}}'       = $Default.Replace("'", "''")
    '{{RANGES}}'              = $rangeText
    '{{MULTILINE_FLAG}}'      = $(if ($Multiline) { 'multiline' } else { 'notmultiline' })
    '{{HIDDEN_FLAG}}'         = $(if ($Hidden) { 'hidden' } else { 'nothidden' })
    '{{APPLICATION}}'         = $Application
    '{{VERSION}}'             = $Version
    '{{INSTALLER}}'           = $Installer
    '{{INSTALLED_DATE}}'      = $installedDate
}

$content = Get-Content -LiteralPath $templatePath -Raw
foreach ($key in $tokens.Keys) {
    $content = $content.Replace($key, [string]$tokens[$key])
}

Set-Content -LiteralPath $targetPath -Value $content -Encoding UTF8
Write-Output $targetPath
