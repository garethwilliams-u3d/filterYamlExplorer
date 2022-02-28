# Filter Yaml Explorer

_Technical writers and developers struggle to write effective exclusion rules in filter.yml and the official documentation is poor. Is there a way that we can speed up or assist writers in better understanding these rules?_

This proof of concept tool attempts to provide a way to speed up the understanding and writing of filter yaml rules by describing how the rules for a given `filter.yml` are applied to a given `assembly`

## Setup

1. Open `<repo root>\docfx\filterExplorer\filterExplorer.csproj` in visual studio 2019
1. Right-click > build filterExplorer project
1. You will find the binary in `<repo root>\docfx\filterExplorer\bin\Debug`

### Usage

```
filterExplorer.exe <path/to/filter.yml> <path/to/class.cs>
```

#### Full usage

Alongside the binary you will find a `\Resources` folder this containing:

- `TestFilter.yaml`
- `TestClass.cs`

`TestFilter.yml` is a basic filter definition that excludes anything in the `CatLib` namespace

`TestClass.cs` is a simple c# class that includes features that can be used to exclude members using each of the [basic filter rule types](https://dotnet.github.io/docfx/tutorial/howto_filter_out_unwanted_apis_attributes.html)

Filter explorer can be run as follows:

```
filterExplorer.exe Resources/TestFilter.yaml Resources/TestClass.cs
```

### Output

You should get a report similar to the following, describing:

- which members were compiled into the assembly from the `class.cs` file
- which members passed through the filter and would be included in the documentation.
- which rules were succesfully applied and why (the index matches the order the rules are defined in the filter.yml)
- If any rules were not applied they are listed along with their definitions.

```
all members in assembly:
IgnoreAttribute
CatLib
DogLib

members included in documentation:
Namespace: DogLib

rule applications:
CatLib.Cat was excluded by user api  rule 0 matched by member uid regex: ^CatLib\.

doc-fx api rule 0 was never applied:
  uid regex:
  kind:
  attribute constructor uid System.ComponentModel.EditorBrowsableAttribute
  attribute constructor args System.ComponentModel.EditorBrowsableState.Never
  attribute named constructor args
doc-fx attribute rule 0 was never applied:
  uid regex: ^System\.ComponentModel\.Design$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 1 was never applied:
  uid regex: ^System\.ComponentModel\.Design\.Serialization$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 2 was never applied:
  uid regex: ^System\.Xml\.Serialization$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 3 was never applied:
  uid regex: ^System\.Web\.Compilation$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 4 was never applied:
  uid regex: ^System\.Runtime\.Versioning$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 5 was never applied:
  uid regex: ^System\.Runtime\.ConstrainedExecution$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 6 was never applied:
  uid regex: ^System\.EnterpriseServices$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 7 was never applied:
  uid regex: ^System\.Diagnostics\.CodeAnalysis$
  kind: Namespace
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 8 was never applied:
  uid regex: ^System\.Diagnostics\.(ConditionalAttribute|EventLogPermissionAttribute|PerformanceCounterPermissionAttribute)$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 9 was never applied:
  uid regex: ^System\.Diagnostics\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 10 was never applied:
  uid regex: ^System\.ComponentModel\.(BindableAttribute|BrowsableAttribute|ComplexBindingPropertiesAttribute|DataObjectAttribute|DefaultBindingPropertyAttribute|ListBindableAttribute|LookupBindingPropertiesAttribute|SettingsBindableAttribute|TypeConverterAttribute)$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 11 was never applied:
  uid regex: ^System\.ComponentModel\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 12 was never applied:
  uid regex: ^System\.Reflection\.DefaultMemberAttribute$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 13 was never applied:
  uid regex: ^System\.CodeDom\.Compiler\.GeneratedCodeAttribute$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 14 was never applied:
  uid regex: ^System\.Runtime\.CompilerServices\.ExtensionAttribute$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 15 was never applied:
  uid regex: ^System\.Runtime\.CompilerServices\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 16 was never applied:
  uid regex: ^System\.Runtime\.InteropServices\.(ComVisibleAttribute|GuidAttribute|ClassInterfaceAttribute|InterfaceTypeAttribute)$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 17 was never applied:
  uid regex: ^System\.Runtime\.InteropServices\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 18 was never applied:
  uid regex: ^System\.Security\.(SecurityCriticalAttribute|SecurityTreatAsSafeAttribute|AllowPartiallyTrustedCallersAttribute)$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 19 was never applied:
  uid regex: ^System\.Security\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 20 was never applied:
  uid regex: ^System\.Web\.UI\.(ControlValuePropertyAttribute|PersistenceModeAttribute|ValidationPropertyAttribute|WebResourceAttribute|TemplateContainerAttribute|ThemeableAttribute|TemplateInstanceAttribute)$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 21 was never applied:
  uid regex: ^System\.Web\.UI\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 22 was never applied:
  uid regex: ^System\.Windows\.Markup\.(ConstructorArgumentAttribute|DesignerSerializationOptionsAttribute|ValueSerializerAttribute|XmlnsCompatibleWithAttribute|XmlnsDefinitionAttribute|XmlnsPrefixAttribute)$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
doc-fx attribute rule 23 was never applied:
  uid regex: ^System\.Windows\.Markup\.[^.]+$
  kind: Type
  attribute constructor uid
  attribute constructor args
  attribute named constructor args
```

#### Pro-tips

- You can edit `Resources/TestClass.cs` in visual studio which will give you compiler support to prevent errors.
- Running filterExplorer > Debug from visual studio will configure filterExplorer to load the two files in resources when launching.
- For quickest iterations you could use Visual Studio IDE to do your explaration, rather than the binary.

## Learnings

See [Implementation details](ImplementationDetails.md)
