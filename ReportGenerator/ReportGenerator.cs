using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilterDebugging
{
    internal class Rule
    {
        internal class AttributeFilter
        {
            //identify attribute by specific ctor uid
            internal string uid; 
            //identify attribute by exact match of specifc args.
            internal List<string> ctorArgs = 
                new List<string>(); 
            //identify attribute by partial match of named args
            internal Dictionary<string, string> namedCtorArgs = 
                new Dictionary<string, string>(); 
        }

        //track how often this rule was applied
        internal int applicationCount;

        //is this a whitelist or blacklist rule ?
        internal bool isInclude;
        //did this come from the user defined yaml
        internal bool userDefined;
        //did this come from the api or attribute section
        internal bool isApiRule;
        //which index from the 
        //user / default list
        //api / attribute section did this come from?
        internal int index = -1;

        //rule
        internal string memberUidRegex; //a regex matching any uid
        internal string kind; //cf "Type" if was type rule
        internal AttributeFilter attribute = new AttributeFilter();
    }

    internal class ReportRow
    {
        internal int ruleId;

        internal bool matchedMemberUid;
        internal bool matchedKind;

        internal bool matchedAttributeUid;
        internal bool matchedAttributeConstructorArgs;
        internal bool matchedAttributeNamedConstructorArgs;
    }

    public class ReportGenerator
    {
        private List<Rule> rules = new List<Rule>();
        private Dictionary<string, ReportRow> uidToReportRow = new Dictionary<string, ReportRow>();

        private static ReportGenerator instance = new ReportGenerator();
        public static ReportGenerator Instance { get => instance; }

        private ReportGenerator() {}

        public void RecordMatch(
            string symbolUid, 
            int ruleId,
            bool matchedMemberUid,
            bool matchedKind
        ) {
            if(false == uidToReportRow.ContainsKey(symbolUid))
            {
                uidToReportRow.Add(symbolUid, new ReportRow() 
                {
                    ruleId = ruleId
                });
            }

            //record match details
            var row = uidToReportRow[symbolUid];
            row.matchedMemberUid = matchedMemberUid;
            row.matchedKind = matchedKind;

            //track matches
            if(matchedMemberUid || matchedKind) rules[ruleId].applicationCount++;
        }

        //TODO: assumes that a record was already created
        public void RecordMatch(
            string symbolUid,
            bool matchedAttributeUid,
            bool matchedAttributeConstructorArgs,
            bool matchedAttributeNamedConstructorArgs
        ) {
            //record match details
            var row = uidToReportRow[symbolUid];
            row.matchedAttributeUid = matchedAttributeUid;
            row.matchedAttributeConstructorArgs = matchedAttributeConstructorArgs;
            row.matchedAttributeNamedConstructorArgs = matchedAttributeNamedConstructorArgs;

            //track matches:
            if (matchedAttributeUid
                || matchedAttributeConstructorArgs
                || matchedAttributeNamedConstructorArgs
            )
            {
                rules[row.ruleId].applicationCount++;
            }
        }

        public int AddRule
        (
            bool userDefined,
            bool isApiRule,
            bool isIncludeRule,
            int index
        ) {
            var id = rules.Count;
            rules.Add(new Rule()
            {
                userDefined = userDefined,
                isApiRule = isApiRule,
                isInclude = isIncludeRule,
                index = index
            });
            return id;
        }

        public void addMemberUidRegex(int ruleId, string regex)
        {
            rules[ruleId].memberUidRegex = regex;
        }

        public void AddKind(int ruleId, string kind)
        {
            rules[ruleId].kind = kind;
        }

        public void AddAttribute(
            int ruleId, 
            string uid,
            List<string> constructorArgs = null,
            Dictionary<string, string> constructorNamedArgs = null
        )
        {
            rules[ruleId].attribute.uid = uid;
            if (constructorArgs != null)
            {
                rules[ruleId].attribute.ctorArgs = constructorArgs;
            }
            if (constructorNamedArgs != null)
            {
                rules[ruleId].attribute.namedCtorArgs = constructorNamedArgs;
            }
        }

        public string GetReport()
        {
            var sb = new StringBuilder();
            foreach(var row in uidToReportRow)
            {
                var uid = row.Key;
                var data = row.Value;
                var rule = rules[data.ruleId];
                
                //template fragments
                var included = rule.isInclude ? "included" : "excluded";
                var origin = 
                    (rule.userDefined ? "user" : "doc-fx") 
                    + " " 
                    + (rule.isApiRule ? "api" : "attribute");

                var matchDescription = new List<string>();
                if (data.matchedMemberUid) matchDescription.Add($"member uid regex: {rule.memberUidRegex}");
                if (data.matchedKind) matchDescription.Add($"kind (type): {rule.kind}");
                if (data.matchedAttributeUid) matchDescription.Add($"attribute constructor uid: {rule.attribute.uid}");
                if (data.matchedAttributeConstructorArgs) matchDescription.Add($"ALL attribute constructor arguments: {string.Join(",", rule.attribute.ctorArgs)}");
                if (data.matchedAttributeNamedConstructorArgs) matchDescription.Add($"SOME named attribute constructor arguments {string.Join(", ", rule.attribute.namedCtorArgs)}");
                var filter = string.Join(" and ", matchDescription);

                sb.Append($"{uid} was {included} by {origin}  rule {rule.index} ");
                sb.Append($"matched by {filter}");

                sb.AppendLine();
            }

            sb.AppendLine();

            foreach(var rule in rules.Where(r => r.applicationCount == 0))
            {
                var origin =
                    (rule.userDefined ? "user" : "doc-fx")
                    + " "  
                    + (rule.isApiRule ? "api" : "attribute");

                var attrCtorArgs = string.Join(",", rule.attribute.ctorArgs);
                var attrCtorNamedArgs = string.Join(
                    ",",
                    rule.attribute.namedCtorArgs
                    .Select(r => $"{r.Key} = {r.Value}")
                );

                sb.AppendLine($"{origin} rule {rule.index} was never applied:");
                sb.AppendLine($"  uid regex: {rule.memberUidRegex}");
                sb.AppendLine($"  kind: {rule.kind}");
                sb.AppendLine($"  attribute constructor uid {rule.attribute.uid}");
                sb.AppendLine($"  attribute constructor args {attrCtorArgs}");
                sb.AppendLine($"  attribute named constructor args {attrCtorNamedArgs}");
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
