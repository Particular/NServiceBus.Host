﻿// ReSharper disable ConvertToLambdaExpression
// ReSharper disable UnusedParameter.Local
// ReSharper disable JoinDeclarationAndInitializer
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable ConvertToAutoProperty
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable RedundantToStringCallForValueType
// ReSharper disable ForCanBeConvertedToForeach
// ReSharper disable RedundantArrayCreationExpression
// ReSharper disable RedundantExplicitArrayCreation
//
// Options.cs
//
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
//
// Copyright (C) 2008 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// Compile With:
//   gmcs -debug+ -r:System.Core Options.cs -o:NDesk.Options.dll
//   gmcs -debug+ -d:LINQ -r:System.Core Options.cs -o:NDesk.Options.dll
//
// The LINQ version just changes the implementation of
// OptionSet.Parse(IEnumerable<string>), and confers no semantic changes.

//
// A Getopt::Long-inspired option parsing library for C#.
//
// NDesk.Options.OptionSet is built upon a key/value table, where the
// key is a option format string and the value is a delegate that is 
// invoked when the format string is matched.
//
// Option format strings:
//  Regex-like BNF Grammar: 
//    name: .+
//    type: [=:]
//    sep: ( [^{}]+ | '{' .+ '}' )?
//    aliases: ( name type sep ) ( '|' name type sep )*
// 
// Each '|'-delimited name is an alias for the associated action.  If the
// format string ends in a '=', it has a required value.  If the format
// string ends in a ':', it has an optional value.  If neither '=' or ':'
// is present, no value is supported.  `=' or `:' need only be defined on one
// alias, but if they are provided on more than one they must be consistent.
//
// Each alias portion may also end with a "key/value separator", which is used
// to split option values if the option accepts > 1 value.  If not specified,
// it defaults to '=' and ':'.  If specified, it can be any character except
// '{' and '}' OR the *string* between '{' and '}'.  If no separator should be
// used (i.e. the separate values should be distinct arguments), then "{}"
// should be used as the separator.
//
// Options are extracted either from the current option by looking for
// the option name followed by an '=' or ':', or is taken from the
// following option IFF:
//  - The current option does not contain a '=' or a ':'
//  - The current option requires a value (i.e. not a Option type of ':')
//
// The `name' used in the option format string does NOT include any leading
// option indicator, such as '-', '--', or '/'.  All three of these are
// permitted/required on any named option.
//
// Option bundling is permitted so long as:
//   - '-' is used to start the option group
//   - all of the bundled options are a single character
//   - at most one of the bundled options accepts a value, and the value
//     provided starts from the next character to the end of the string.
//
// This allows specifying '-a -b -c' as '-abc', and specifying '-D name=value'
// as '-Dname=value'.
//
// Option processing is disabled by specifying "--".  All options after "--"
// are returned by OptionSet.Parse() unchanged and unprocessed.
//
// Unprocessed options are returned from OptionSet.Parse().
//
// Examples:
//  int verbose = 0;
//  OptionSet p = new OptionSet ()
//    .Add ("v", v => ++verbose)
//    .Add ("name=|value=", v => Console.WriteLine (v));
//  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"});
//
// The above would parse the argument string array, and would invoke the
// lambda expression three times, setting `verbose' to 3 when complete.  
// It would also print out "A" and "B" to standard output.
// The returned array would contain the string "extra".
//
// C# 3.0 collection initializers are supported and encouraged:
//  var p = new OptionSet () {
//    { "h|?|help", v => ShowHelp () },
//  };
//
// System.ComponentModel.TypeConverter is also supported, allowing the use of
// custom data types in the callback type; TypeConverter.ConvertFromString()
// is used to convert the value option to an instance of the specified
// type:
//
//  var p = new OptionSet () {
//    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
//  };
//
// Random other tidbits:
//  - Boolean options (those w/o '=' or ':' in the option format string)
//    are explicitly enabled if they are followed with '+', and explicitly
//    disabled if they are followed with '-':
//      string a = null;
//      var p = new OptionSet () {
//        { "a", s => a = s },
//      };
//      p.Parse (new string[]{"-a"});   // sets v != null
//      p.Parse (new string[]{"-a+"});  // sets v != null
//      p.Parse (new string[]{"-a-"});  // sets v == null
//


#if LINQ
using System.Linq;
#endif

#if TEST
using NDesk.Options;
#endif

namespace NDesk.Options
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.RegularExpressions;

    class OptionValueCollection : IList, IList<string>
    {

        List<string> values = new List<string>();
        OptionContext c;

        internal OptionValueCollection(OptionContext c)
        {
            this.c = c;
        }

        #region ICollection
        void ICollection.CopyTo(Array array, int index) { (values as ICollection).CopyTo(array, index); }
        bool ICollection.IsSynchronized => (values as ICollection).IsSynchronized;
        object ICollection.SyncRoot => (values as ICollection).SyncRoot;

        #endregion

        #region ICollection<T>
        public void Add(string item) { values.Add(item); }
        public void Clear() { values.Clear(); }
        public bool Contains(string item) { return values.Contains(item); }
        public void CopyTo(string[] array, int arrayIndex) { values.CopyTo(array, arrayIndex); }
        public bool Remove(string item) { return values.Remove(item); }
        public int Count => values.Count;
        public bool IsReadOnly => false;

        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator() { return values.GetEnumerator(); }
        #endregion

        #region IEnumerable<T>
        public IEnumerator<string> GetEnumerator() { return values.GetEnumerator(); }
        #endregion

        #region IList
        int IList.Add(object value) { return (values as IList).Add(value); }
        bool IList.Contains(object value) { return (values as IList).Contains(value); }
        int IList.IndexOf(object value) { return (values as IList).IndexOf(value); }
        void IList.Insert(int index, object value) { (values as IList).Insert(index, value); }
        void IList.Remove(object value) { (values as IList).Remove(value); }
        void IList.RemoveAt(int index) { (values as IList).RemoveAt(index); }
        bool IList.IsFixedSize => false;
        object IList.this[int index] { get { return this[index]; } set { (values as IList)[index] = value; } }
        #endregion

        #region IList<T>
        public int IndexOf(string item) { return values.IndexOf(item); }
        public void Insert(int index, string item) { values.Insert(index, item); }
        public void RemoveAt(int index) { values.RemoveAt(index); }

        void AssertValid(int index)
        {
            if (c.Option == null)
                throw new InvalidOperationException("OptionContext.Option is null.");
            if (index >= c.Option.MaxValueCount)
                throw new ArgumentOutOfRangeException("index");
            if (c.Option.OptionValueType == OptionValueType.Required &&
                    index >= values.Count)
                throw new OptionException(string.Format(
                            c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName),
                        c.OptionName);
        }

        public string this[int index]
        {
            get
            {
                AssertValid(index);
                return index >= values.Count ? null : values[index];
            }
            set
            {
                values[index] = value;
            }
        }
        #endregion

        public List<string> ToList()
        {
            return new List<string>(values);
        }

        public string[] ToArray()
        {
            return values.ToArray();
        }

        public override string ToString()
        {
            return string.Join(", ", values.ToArray());
        }
    }

    class OptionContext
    {
        OptionSet set;
        OptionValueCollection c;

        public OptionContext(OptionSet set)
        {
            this.set = set;
            c = new OptionValueCollection(this);
        }

        public Option Option { get; set; }

        public string OptionName { get; set; }

        public int OptionIndex { get; set; }

        public OptionSet OptionSet => set;

        public OptionValueCollection OptionValues => c;
    }

    enum OptionValueType
    {
        None,
        Optional,
        Required,
    }

    abstract class Option
    {
        string prototype, description;
        string[] names;
        OptionValueType type;
        int count;
        string[] separators;

        protected Option(string prototype, string description)
            : this(prototype, description, 1)
        {
        }

        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
                throw new ArgumentNullException(nameof(prototype));
            if (prototype.Length == 0)
                throw new ArgumentException("Cannot be the empty string.", nameof(prototype));
            if (maxValueCount < 0)
                throw new ArgumentOutOfRangeException(nameof(maxValueCount));

            this.prototype = prototype;
            names = prototype.Split('|');
            this.description = description;
            count = maxValueCount;
            type = ParsePrototype();

            if (count == 0 && type != OptionValueType.None)
                throw new ArgumentException(
                        "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
                            "OptionValueType.Optional.",
                        nameof(maxValueCount));
            if (type == OptionValueType.None && maxValueCount > 1)
                throw new ArgumentException(
                    $"Cannot provide maxValueCount of {maxValueCount} for OptionValueType.None.",
                        nameof(maxValueCount));
            if (Array.IndexOf(names, "<>") >= 0 &&
                    ((names.Length == 1 && type != OptionValueType.None) ||
                     (names.Length > 1 && MaxValueCount > 1)))
                throw new ArgumentException(
                        "The default option handler '<>' cannot require values.",
                        nameof(prototype));
        }

        public string Prototype => prototype;
        public string Description => description;
        public OptionValueType OptionValueType => type;
        public int MaxValueCount => count;

        public string[] GetNames()
        {
            return (string[])names.Clone();
        }

        public string[] GetValueSeparators()
        {
            if (separators == null)
                return new string[0];
            return (string[])separators.Clone();
        }

        protected static T Parse<T>(string value, OptionContext c)
        {
            var conv = TypeDescriptor.GetConverter(typeof(T));
            var t = default(T);
            try
            {
                if (value != null)
                    t = (T)conv.ConvertFromString(value);
            }
            catch (Exception e)
            {
                throw new OptionException(
                        string.Format(
                            c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."),
                            value, typeof(T).Name, c.OptionName),
                        c.OptionName, e);
            }
            return t;
        }

        internal string[] Names => names;
        internal string[] ValueSeparators => separators;

        static readonly char[] NameTerminator = new char[] { '=', ':' };

        OptionValueType ParsePrototype()
        {
            var type = '\0';
            var seps = new List<string>();
            for (var i = 0; i < names.Length; ++i)
            {
                var name = names[i];
                if (name.Length == 0)
                    throw new ArgumentException("Empty option names are not supported.");

                var end = name.IndexOfAny(NameTerminator);
                if (end == -1)
                    continue;
                names[i] = name.Substring(0, end);
                if (type == '\0' || type == name[end])
                    type = name[end];
                else
                    throw new ArgumentException(
                        $"Conflicting option types: '{type}' vs. '{name[end]}'.");
                AddSeparators(name, end, seps);
            }

            if (type == '\0')
                return OptionValueType.None;

            if (count <= 1 && seps.Count != 0)
                throw new ArgumentException(
                    $"Cannot provide key/value separators for Options taking {count} value(s).");
            if (count > 1)
            {
                if (seps.Count == 0)
                    separators = new[] { ":", "=" };
                else if (seps.Count == 1 && seps[0].Length == 0)
                    separators = null;
                else
                    separators = seps.ToArray();
            }

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }

        static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            var start = -1;
            for (var i = end + 1; i < name.Length; ++i)
            {
                switch (name[i])
                {
                    case '{':
                        if (start != -1)
                            throw new ArgumentException(
                                $"Ill-formed name/value separator found in \"{name}\".");
                        start = i + 1;
                        break;
                    case '}':
                        if (start == -1)
                            throw new ArgumentException(
                                $"Ill-formed name/value separator found in \"{name}\".");
                        seps.Add(name.Substring(start, i - start));
                        start = -1;
                        break;
                    default:
                        if (start == -1)
                            seps.Add(name[i].ToString());
                        break;
                }
            }
            if (start != -1)
                throw new ArgumentException(
                    $"Ill-formed name/value separator found in \"{name}\".");
        }

        public void Invoke(OptionContext c)
        {
            OnParseComplete(c);
            c.OptionName = null;
            c.Option = null;
            c.OptionValues.Clear();
        }

        protected abstract void OnParseComplete(OptionContext c);

        public override string ToString()
        {
            return Prototype;
        }
    }

    [Serializable]
    class OptionException : Exception
    {
        string option;

        public OptionException()
        {
        }

        public OptionException(string message, string optionName)
            : base(message)
        {
            option = optionName;
        }

        public OptionException(string message, string optionName, Exception innerException)
            : base(message, innerException)
        {
            option = optionName;
        }

        protected OptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            option = info.GetString("OptionName");
        }

        public string OptionName => option;

        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OptionName", option);
        }
    }

    delegate void OptionAction<TKey, TValue>(TKey key, TValue value);

    class OptionSet : KeyedCollection<string, Option>
    {
        public OptionSet()
            : this(f => f)
        {
        }

        public OptionSet(Converter<string, string> localizer)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            this.localizer = localizer;
        }

        Converter<string, string> localizer;

        public Converter<string, string> MessageLocalizer => localizer;

        protected override string GetKeyForItem(Option item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (item.Names != null && item.Names.Length > 0)
                return item.Names[0];
            // This should never happen, as it's invalid for Option to be
            // constructed w/o any names.
            throw new InvalidOperationException("Option has no names!");
        }

        protected override void InsertItem(int index, Option item)
        {
            base.InsertItem(index, item);
            AddImpl(item);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            var p = Items[index];
            // KeyedCollection.RemoveItem() handles the 0th item
            for (var i = 1; i < p.Names.Length; ++i)
            {
                Dictionary.Remove(p.Names[i]);
            }
        }

        protected override void SetItem(int index, Option item)
        {
            base.SetItem(index, item);
            RemoveItem(index);
            AddImpl(item);
        }

        void AddImpl(Option option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            var added = new List<string>(option.Names.Length);
            try
            {
                // KeyedCollection.InsertItem/SetItem handle the 0th name.
                for (var i = 1; i < option.Names.Length; ++i)
                {
                    Dictionary.Add(option.Names[i], option);
                    added.Add(option.Names[i]);
                }
            }
            catch (Exception)
            {
                foreach (var name in added)
                    Dictionary.Remove(name);
                throw;
            }
        }

        public new OptionSet Add(Option option)
        {
            base.Add(option);
            return this;
        }

        sealed class ActionOption : Option
        {
            Action<OptionValueCollection> action;

            public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action)
                : base(prototype, description, count)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                action(c.OptionValues);
            }
        }

        public OptionSet Add(string prototype, Action<string> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            Option p = new ActionOption(prototype, description, 1,
                    delegate(OptionValueCollection v) { action(v[0]); });
            base.Add(p);
            return this;
        }

        public OptionSet Add(string prototype, OptionAction<string, string> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add(string prototype, string description, OptionAction<string, string> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            Option p = new ActionOption(prototype, description, 2,
                    delegate(OptionValueCollection v) { action(v[0], v[1]); });
            base.Add(p);
            return this;
        }

        sealed class ActionOption<T> : Option
        {
            Action<T> action;

            public ActionOption(string prototype, string description, Action<T> action)
                : base(prototype, description, 1)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                action(Parse<T>(c.OptionValues[0], c));
            }
        }

        sealed class ActionOption<TKey, TValue> : Option
        {
            OptionAction<TKey, TValue> action;

            public ActionOption(string prototype, string description, OptionAction<TKey, TValue> action)
                : base(prototype, description, 2)
            {
                if (action == null)
                    throw new ArgumentNullException(nameof(action));
                this.action = action;
            }

            protected override void OnParseComplete(OptionContext c)
            {
                action(
                        Parse<TKey>(c.OptionValues[0], c),
                        Parse<TValue>(c.OptionValues[1], c));
            }
        }

        public OptionSet Add<T>(string prototype, Action<T> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add<T>(string prototype, string description, Action<T> action)
        {
            return Add(new ActionOption<T>(prototype, description, action));
        }

        public OptionSet Add<TKey, TValue>(string prototype, OptionAction<TKey, TValue> action)
        {
            return Add(prototype, null, action);
        }

        public OptionSet Add<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
        {
            return Add(new ActionOption<TKey, TValue>(prototype, description, action));
        }

        protected virtual OptionContext CreateOptionContext()
        {
            return new OptionContext(this);
        }

#if LINQ
		public List<string> Parse (IEnumerable<string> arguments)
		{
			bool process = true;
			OptionContext c = CreateOptionContext ();
			c.OptionIndex = -1;
			var def = GetOptionForName ("<>");
			var unprocessed = 
				from argument in arguments
				where ++c.OptionIndex >= 0 && (process || def != null)
					? process
						? argument == "--" 
							? (process = false)
							: !Parse (argument, c)
								? def != null 
									? Unprocessed (null, def, c, argument) 
									: true
								: false
						: def != null 
							? Unprocessed (null, def, c, argument)
							: true
					: true
				select argument;
			List<string> r = unprocessed.ToList ();
			if (c.Option != null)
				c.Option.Invoke (c);
			return r;
		}
#else
        public List<string> Parse(IEnumerable<string> arguments)
        {
            var c = CreateOptionContext();
            c.OptionIndex = -1;
            var process = true;
            var unprocessed = new List<string>();
            var def = Contains("<>") ? this["<>"] : null;
            foreach (var argument in arguments)
            {
                ++c.OptionIndex;
                if (argument == "--")
                {
                    process = false;
                    continue;
                }
                if (!process)
                {
                    Unprocessed(unprocessed, def, c, argument);
                    continue;
                }
                if (!Parse(argument, c))
                    Unprocessed(unprocessed, def, c, argument);
            }
            c.Option?.Invoke(c);
            return unprocessed;
        }
#endif

        static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
        {
            if (def == null)
            {
                extra.Add(argument);
                return false;
            }
            c.OptionValues.Add(argument);
            c.Option = def;
            c.Option.Invoke(c);
            return false;
        }

        readonly Regex ValueOption = new Regex(
            "^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

        protected bool GetOptionParts(string argument, out string flag, out string name, out string sep, out string value)
        {
            if (argument == null)
                throw new ArgumentNullException(nameof(argument));

            flag = name = sep = value = null;
            var m = ValueOption.Match(argument);
            if (!m.Success)
            {
                return false;
            }
            flag = m.Groups["flag"].Value;
            name = m.Groups["name"].Value;
            if (m.Groups["sep"].Success && m.Groups["value"].Success)
            {
                sep = m.Groups["sep"].Value;
                value = m.Groups["value"].Value;
            }
            return true;
        }

        protected virtual bool Parse(string argument, OptionContext c)
        {
            if (c.Option != null)
            {
                ParseValue(argument, c);
                return true;
            }

            string f, n, s, v;
            if (!GetOptionParts(argument, out f, out n, out s, out v))
                return false;

            if (Contains(n))
            {
                var p = this[n];
                c.OptionName = f + n;
                c.Option = p;
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        c.OptionValues.Add(n);
                        c.Option.Invoke(c);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                        ParseValue(v, c);
                        break;
                }
                return true;
            }
            // no match; is it a bool option?
            if (ParseBool(argument, n, c))
                return true;
            // is it a bundled option?
            if (ParseBundledValue(f, string.Concat(n + s + v), c))
                return true;

            return false;
        }

        void ParseValue(string option, OptionContext c)
        {
            if (option != null)
                foreach (var o in c.Option.ValueSeparators != null
                        ? option.Split(c.Option.ValueSeparators, StringSplitOptions.None)
                        : new[] { option })
                {
                    c.OptionValues.Add(o);
                }
            if (c.OptionValues.Count == c.Option.MaxValueCount ||
                    c.Option.OptionValueType == OptionValueType.Optional)
                c.Option.Invoke(c);
            else if (c.OptionValues.Count > c.Option.MaxValueCount)
            {
                throw new OptionException(localizer($"Error: Found {c.OptionValues.Count} option values when expecting {c.Option.MaxValueCount}."),
                        c.OptionName);
            }
        }

        bool ParseBool(string option, string n, OptionContext c)
        {
            string rn;
            if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
                    Contains((rn = n.Substring(0, n.Length - 1))))
            {
                var p = this[rn];
                var v = n[n.Length - 1] == '+' ? option : null;
                c.OptionName = option;
                c.Option = p;
                c.OptionValues.Add(v);
                p.Invoke(c);
                return true;
            }
            return false;
        }

        bool ParseBundledValue(string f, string n, OptionContext c)
        {
            if (f != "-")
                return false;
            for (var i = 0; i < n.Length; ++i)
            {
                Option p;
                var opt = f + n[i].ToString();
                var rn = n[i].ToString();
                if (!Contains(rn))
                {
                    if (i == 0)
                        return false;
                    throw new OptionException(string.Format(localizer(
                                    "Cannot bundle unregistered option '{0}'."), opt), opt);
                }
                p = this[rn];
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        Invoke(c, opt, n, p);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                        {
                            var v = n.Substring(i + 1);
                            c.Option = p;
                            c.OptionName = opt;
                            ParseValue(v.Length != 0 ? v : null, c);
                            return true;
                        }
                    default:
                        throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
                }
            }
            return true;
        }

        static void Invoke(OptionContext c, string name, string value, Option option)
        {
            c.OptionName = name;
            c.Option = option;
            c.OptionValues.Add(value);
            option.Invoke(c);
        }

        const int OptionWidth = 29;

        public void WriteOptionDescriptions(TextWriter o)
        {
            foreach (var p in this)
            {
                var written = 0;
                if (!WriteOptionPrototype(o, p, ref written))
                    continue;

                if (written < OptionWidth)
                    o.Write(new string(' ', OptionWidth - written));
                else
                {
                    o.WriteLine();
                    o.Write(new string(' ', OptionWidth));
                }

                var lines = GetLines(localizer(GetDescription(p.Description)));
                o.WriteLine(lines[0]);
                var prefix = new string(' ', OptionWidth + 2);
                for (var i = 1; i < lines.Count; ++i)
                {
                    o.Write(prefix);
                    o.WriteLine(lines[i]);
                }
            }
        }

        bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
        {
            var names = p.Names;

            var i = GetNextOptionIndex(names, 0);
            if (i == names.Length)
                return false;

            if (names[i].Length == 1)
            {
                Write(o, ref written, "  -");
                Write(o, ref written, names[0]);
            }
            else
            {
                Write(o, ref written, "  -");
                Write(o, ref written, names[0]);
            }

            for (i = GetNextOptionIndex(names, i + 1);
                    i < names.Length; i = GetNextOptionIndex(names, i + 1))
            {
                Write(o, ref written, ", ");
                Write(o, ref written, names[i].Length == 1 ? "-" : "--");
                Write(o, ref written, names[i]);
            }

            if (p.OptionValueType == OptionValueType.Optional ||
                    p.OptionValueType == OptionValueType.Required)
            {
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("["));
                }
                Write(o, ref written, localizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
                var sep = p.ValueSeparators != null && p.ValueSeparators.Length > 0
                    ? p.ValueSeparators[0]
                    : " ";
                for (var c = 1; c < p.MaxValueCount; ++c)
                {
                    Write(o, ref written, localizer(sep + GetArgumentName(c, p.MaxValueCount, p.Description)));
                }
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("]"));
                }
            }
            return true;
        }

        static int GetNextOptionIndex(string[] names, int i)
        {
            while (i < names.Length && names[i] == "<>")
            {
                ++i;
            }
            return i;
        }

        static void Write(TextWriter o, ref int n, string s)
        {
            n += s.Length;
            o.Write(s);
        }

        static string GetArgumentName(int index, int maxIndex, string description)
        {
            if (description == null)
                return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
            string[] nameStart;
            if (maxIndex == 1)
                nameStart = new[] { "{0:", "{" };
            else
                nameStart = new[] { "{" + index + ":" };
            for (var i = 0; i < nameStart.Length; ++i)
            {
                int start, j = 0;
                do
                {
                    start = description.IndexOf(nameStart[i], j);
                } while (start >= 0 && j != 0 ? description[j++ - 1] == '{' : false);
                if (start == -1)
                    continue;
                var end = description.IndexOf("}", start);
                if (end == -1)
                    continue;
                return description.Substring(start + nameStart[i].Length, end - start - nameStart[i].Length);
            }
            return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
        }

        static string GetDescription(string description)
        {
            if (description == null)
                return string.Empty;
            var sb = new StringBuilder(description.Length);
            var start = -1;
            for (var i = 0; i < description.Length; ++i)
            {
                switch (description[i])
                {
                    case '{':
                        if (i == start)
                        {
                            sb.Append('{');
                            start = -1;
                        }
                        else if (start < 0)
                            start = i + 1;
                        break;
                    case '}':
                        if (start < 0)
                        {
                            if ((i + 1) == description.Length || description[i + 1] != '}')
                                throw new InvalidOperationException("Invalid option description: " + description);
                            ++i;
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append(description.Substring(start, i - start));
                            start = -1;
                        }
                        break;
                    case ':':
                        if (start < 0)
                            goto default;
                        start = i + 1;
                        break;
                    default:
                        if (start < 0)
                            sb.Append(description[i]);
                        break;
                }
            }
            return sb.ToString();
        }

        static List<string> GetLines(string description)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(description))
            {
                lines.Add(string.Empty);
                return lines;
            }
            var length = 80 - OptionWidth - 2;
            int start = 0, end;
            do
            {
                end = GetLineEnd(start, length, description);
                var cont = false;
                if (end < description.Length)
                {
                    var c = description[end];
                    if (c == '-' || (char.IsWhiteSpace(c) && c != '\n'))
                        ++end;
                    else if (c != '\n')
                    {
                        cont = true;
                        --end;
                    }
                }
                lines.Add(description.Substring(start, end - start));
                if (cont)
                {
                    lines[lines.Count - 1] += "-";
                }
                start = end;
                if (start < description.Length && description[start] == '\n')
                    ++start;
            } while (end < description.Length);
            return lines;
        }

        static int GetLineEnd(int start, int length, string description)
        {
            var end = Math.Min(start + length, description.Length);
            var sep = -1;
            for (var i = start; i < end; ++i)
            {
                switch (description[i])
                {
                    case ' ':
                    case '\t':
                    case '\v':
                    case '-':
                    case ',':
                    case '.':
                    case ';':
                        sep = i;
                        break;
                    case '\n':
                        return i;
                }
            }
            if (sep == -1 || end == description.Length)
                return end;
            return sep;
        }
    }
}