################################################################################################
# Created Date : {{CREATED_DATE}}
# Create by {{CREATED_BY}}
# Description : {{DESCRIPTION_KO}}
################################################################################################
#del attribute '{{ATTRIBUTE_NAME}}';
add attribute '{{ATTRIBUTE_NAME}}'
    description '{{DESCRIPTION_EN}}'{{DESCRIPTION_COMMENT}}
    type '{{ATTRIBUTE_TYPE}}'
    default '{{DEFAULT_VALUE}}'
{{RANGES}}    {{MULTILINE_FLAG}}
    {{HIDDEN_FLAG}}
    property    'application'    value '{{APPLICATION}}'
    property    'version'        value '{{VERSION}}'
    property    'installer'      value '{{INSTALLER}}'
    property    'installed date' value '{{INSTALLED_DATE}}'
    property    'original name'  value '{{ATTRIBUTE_NAME}}'
;
add property 'attribute_{{ATTRIBUTE_NAME}}' on program eServiceSchemaVariableMapping.tcl to attribute '{{ATTRIBUTE_NAME}}';
