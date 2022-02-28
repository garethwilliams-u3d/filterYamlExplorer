# Implementation Details

I have forked the doc-fx source to gather data at the key-points where the filtering is applied to members and then present it in a human-readable report at the end. I have isolated the code that parses the `assembly` and applies the `filter.yml` in what is effectively a test-fixture in `filterExplorer/program.cs`

The key-points where filtering is applied are listed below

## How filter.yml rules are applied by doc-fx

### Visitor

Each Type in the Assembly is [visited](https://joshvarty.com/2015/10/25/learn-roslyn-now-part-15-the-symbolvisitor/) by [SymbolFilterVisitorAdapter](https://github.com/garethwilliams-u3d/filterYamlExplorer/blob/f3b398e5604573eb696602ce5e0339e790dbd12c/src/Microsoft.DocAsCode.Metadata.ManagedReference.Roslyn/Visitors/SymbolVisitorAdapter.cs#L61)

Only the following types will be visited (meaning documentation will only be produced for the following types)

- Assembly
- Namespace
- NamedType (IE Class or Struct - anything explicitly typed)
- Method
- Field
- Event
- Property

Note: This list doesn't include NamedSymbols, so Attribute _definitions_ (the class defining the attribute itself) will not be documented.

### Filter

- A list of user rules are deserialised from the user `filter.yml`
- The [rules are prepended to the list of default rules](https://github.com/garethwilliams-u3d/filterYamlExplorer/blob/f3b398e5604573eb696602ce5e0339e790dbd12c/src/Microsoft.DocAsCode.Metadata.ManagedReference.Common/Filters/ConfigFilterRule.cs#L92) from doc-fx
- The [whole list of rules is iterated per type visited](https://github.com/garethwilliams-u3d/filterYamlExplorer/blob/f3b398e5604573eb696602ce5e0339e790dbd12c/src/Microsoft.DocAsCode.Metadata.ManagedReference.Common/Filters/ConfigFilterRule.cs#L38)
- If any rule qualifies a type to be visited (in the case of inclusion) the iteration stops and no further rules are evaluated; For exclusion iteration stops when a rule disqualifies a type from being visited.

From the above we can see that:

- Since user rules are evaluated earlier in the list they take precedence over them
- Since the first rule to qualify or disqualify a member stops iteration, in the case of overlapping rules the earliest one 'wins'

### Rules

[A rule qualifies or disqualifies a member to be visited](https://github.com/garethwilliams-u3d/filterYamlExplorer/blob/f3b398e5604573eb696602ce5e0339e790dbd12c/src/Microsoft.DocAsCode.Metadata.ManagedReference.Common/Filters/ConfigFilterRuleItem.cs#L49) if:

```
no regex is defined for the rule OR the regex matches the uid of the member
AND no type is defined for the rule OR the rules type matches the type of the member
AND no attribute is defined for the rule OR the rules attribute matches the attribute of the member
```

From the above we can see:

- uid regex is the highest priority rule and is valid in combination with both kind (type) and attribute rules.
- conversely, a kind rule or an attribute rule will both fail if the uid regex is defined and fails.

### See also

[All of the above is documented in the doc-fx docs](https://dotnet.github.io/docfx/tutorial/howto_filter_out_unwanted_apis_attributes.html) but I don't think it is communicated very clearly.
