// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Metadata.ManagedReference
{
    using FilterDebugging;
    using System;
    using System.Text.RegularExpressions;

    using YamlDotNet.Serialization;

    public abstract class ConfigFilterRuleItem
    {
        [YamlIgnore]
        public int id = -1;

        private Regex _uidRegex;

        [YamlMember(Alias = "uidRegex")]
        public string UidRegex
        {
            get
            {
                return _uidRegex?.ToString();
            }
            set
            {
                _uidRegex = new Regex(value);
            }
        }

        [YamlMember(Alias = "type")]
        public ExtendedSymbolKind? Kind { get; set; }

        [YamlMember(Alias = "hasAttribute")]
        public AttributeFilterInfo Attribute { get; set; }

        [YamlIgnore]
        public abstract bool CanVisit { get; }

        public bool IsMatch(SymbolFilterData symbol)
        {
            if (symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            var id = symbol.Id;

            var isMatch = (_uidRegex == null || (id != null && _uidRegex.IsMatch(id))) &&
                (Kind == null || Kind.Value.Contains(symbol)) &&
                (Attribute == null || Attribute.ContainedIn(symbol));

            if (id != null && isMatch)
            {
                var uidRegexMatch = UidRegex != null ? _uidRegex.IsMatch(id) : false;
                var kindMatch = Kind != null ? Kind.Value.Contains(symbol) : false;
                ReportGenerator.Instance.RecordMatch(
                    symbolUid: symbol.Id,
                    ruleId: this.id,
                    matchedMemberUid: uidRegexMatch,
                    matchedKind: kindMatch
                );
            }
            
            return isMatch;
        }
    }
}
