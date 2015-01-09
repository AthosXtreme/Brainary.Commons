namespace Brainary.Commons.Web.DataTables
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Routing;
    using System.Web.Script.Serialization;

    using Brainary.Commons.Extensions;

    public class DataTableViewModel
    {
        public const string DefaultDomTemplate = "{0}{1}{2}rtip";
        public const string BootstrapDomTemplate = "{0}<'row'<'col-xs-6'{1}><'col-xs-6'{2}>><'row'<'col-sm-12'tr>><'row'<'col-xs-6'i><'col-xs-6'p>>";
        public const string ControlBoxTemplateJqueryUI = "<a href=\"#{editTarget}" + CbDataToken + "\" name=\"editRow\" class=\"ui-state-default ui-corner-all\" title=\"Editar\"><span class=\"ui-icon ui-icon-pencil\"></span></a>&nbsp;&nbsp;<a href=\"#{deleteTarget}" + CbDataToken + "\" name=\"deleteRow\" class=\"ui-state-default ui-corner-all\" title=\"Eliminar\"><span class=\"ui-icon ui-icon-minus\"></span></a>";
        public const string ControlBoxTemplateBootstrap = "<a href=\"#{editTarget}" + CbDataToken + "\" name=\"editRow\" class=\"btn btn-default btn-xs\" title=\"Editar\"><span class=\"glyphicon glyphicon-pencil\"></span></a>&nbsp;&nbsp;<a href=\"#{deleteTarget}" + CbDataToken + "\" name=\"deleteRow\" class=\"btn btn-default btn-xs\" title=\"Eliminar\"><span class=\"glyphicon glyphicon-remove\"></span></a>";
        public const string CbDataToken = "_DATA_";

        private static readonly List<Type> DateTypes;

        static DataTableViewModel()
        {
            DateTypes = new List<Type> { typeof(DateTime), typeof(DateTime?), typeof(DateTimeOffset), typeof(DateTimeOffset?) };
            DefaultTableClass = "table table-bordered table-striped";
            StaticFilterTypeRules = new FilterRuleList
            {
                (c, t) => DateTypes.Contains(t) ? "{type: 'date-range'}" : null,
                (c, t) => t == typeof(bool) ? "{type: 'checkbox', values : ['True', 'False']}" : null,
                (c, t) => t.IsEnum ? ("{type: 'checkbox', values : ['" + string.Join("','", Enum.GetNames(t)) + "']}") : null,
                (c, t) => "{type: 'text'}", // by default, text filter on everything
            };
        }

        public DataTableViewModel(string id, string ajaxUrl, DataTable initialData)
        {
            InitialData = initialData.Rows.Cast<DataRow>().Select(r => initialData.Columns.Cast<DataColumn>().Select(c => DataTablesParser.GetTransformedValue(c.DataType, r[c])).ToArray()).ToArray();
            DisplayLength = InitialData.Count();
            var columns = initialData.Columns.Cast<DataColumn>().Select(c => ColDef.Create(c.ColumnName, null, typeof(string))).ToList();
            Init(id, ajaxUrl, columns);
        }

        public DataTableViewModel(string id, string ajaxUrl, IList<ColDef> columns)
        {
            DisplayLength = 10;
            Init(id, ajaxUrl, columns);
        }

        public static string DefaultTableClass { get; set; }

        public static FilterRuleList StaticFilterTypeRules { get; set; }

        public string TableClass { get; set; }

        public bool ShowSearch { get; set; }

        public string Id { get; private set; }

        public string AjaxUrl { get; private set; }

        public IList<ColDef> Columns { get; private set; }
        
        public int? RowIdIndex { get; private set; }

        public IDictionary<string, object> JsOptions { get; set; }

        public bool ControlBox { get; set; }

        public IDictionary<string, object> CustomData { get; set; }

        public int ControlBoxDataIndex { get; private set; }

        public string ControlBoxDataToken { get; set; }

        public IDictionary<string, string> ControlBoxTokens { get; set; }

        public string ControlBoxTemplate { get; set; }

        public bool TableTools { get; set; }

        public IList<KeyValuePair<string, IDictionary<string, object>>> TableToolsButtons { get; set; }

        public bool AutoWidth { get; set; }

        public bool ColumnFilter { get; set; }

        public bool Sortable { get; set; }

        public object[][] Sort { get; private set; }

        public int[] DeferLoading { get; set; }
        
        public object[][] InitialData { get; internal set; }

        public FilterRuleList FilterTypeRules { get; set; }

        public int DisplayLength { get; set; }

        public string ColumnFilterString
        {
            get
            {
                return string.Join(",", Columns.Select(c => GetFilterType(c.Name, c.Type)));
            }
        }

        public string JsOptionsString
        {
            get
            {
                return ConvertDictionaryToJsonBody(JsOptions);
            }
        }

        public string CustomDataString
        {
            get
            {
                return ConvertDictionaryToJsonBody(CustomData);
            }
        }

        public string VisibleColumnsString
        {
            get { return string.Join(",", Columns.Select((v, i) => new { Index = i, Value = v }).Where(w => !w.Value.Hidden).Select(s => s.Index)); }
        }

        public string HiddenColumnsString
        {
            get { return string.Join(",", Columns.Select((v, i) => new { Index = i, Value = v }).Where(w => w.Value.Hidden).Select(s => s.Index)); }
        }

        public string NonSortableColumnsString
        {
            get { return string.Join(",", Columns.Select((v, i) => new { Index = i, Value = v }).Where(w => !w.Value.Hidden && !w.Value.Sortable).Select(s => s.Index)); }
        }

        public bool ShowPageSizes { get; set; }

        public string RenderDom()
        {
            return RenderDom(DefaultDomTemplate);
        }

        public string RenderDom(string template)
        {
            var tt = TableTools ? "T<\"clear\">" : string.Empty;
            var ps = ShowPageSizes ? "l" : string.Empty;
            var ss = ShowSearch ? "f" : string.Empty;
            return string.Format(template, tt, ps, ss);
        }

        public string RenderControlBox()
        {
            var render = ControlBoxTemplate.Clone().ToString();
            return ControlBoxTokens.Aggregate(render, (current, token) => current.Replace(token.Key, token.Value));
        }

        public void HideColumns(params string[] columnNames)
        {
            var columns = Columns.Where(w => columnNames.Contains(w.Name));
            foreach (var column in columns)
                column.Hidden = true;
        }

        public void SortableColumns(params string[] columnNames)
        {
            var columns = Columns.Where(w => !columnNames.Contains(w.Name));
            foreach (var column in columns)
                column.Sortable = false;
        }

        public void RowIdColumn(string columnName)
        {
            RowIdColumn(Columns.Select((v, i) => new { Index = i, Value = v }).Where(w => w.Value.Name == columnName).Select(s => s.Index).First());
        }

        public void RowIdColumn(int columnIndex)
        {
            RowIdIndex = columnIndex;
        }

        public void ControlBoxDataColumn(string columnName)
        {
            ControlBoxDataColumn(Columns.Select((v, i) => new { Index = i, Value = v }).Where(w => w.Value.Name == columnName).Select(s => s.Index).First());
        }

        public void ControlBoxDataColumn(int columnIndex)
        {
            ControlBoxDataIndex = columnIndex;
        }

        public void SetSorting(IDictionary<string, ListSortDirection> sortCriteria)
        {
            if (!sortCriteria.Any()) return;
            Sort = sortCriteria.Select(d => new object[]
                    {
                        Columns.Select((v, i) => new { Index = i, Value = v }).Where(w => w.Value.Name == d.Key).Select(s => s.Index).First(),
                        d.Value == ListSortDirection.Ascending ? "asc" : "desc"
                    }).ToArray();
        }

        public void SetInitialData<T>(IEnumerable<T> data)
        {
            var properties = typeof(T).GetSortedProperties();
            InitialData = data.Select(i => properties.Select(p => DataTablesParser.GetTransformedValue(p.PropertyType, p.GetGetMethod().Invoke(i, null))).ToArray()).ToArray();
        }

        public string GetFilterType(string columnName, Type type)
        {
            foreach (var rule in FilterTypeRules.Select(filterTypeRule => filterTypeRule(columnName, type)).Where(rule => rule != null))
                return rule;

            return "null";
        }

        public Filter<DataTableViewModel> FilterOn<T>()
        {
            return FilterOn<T>(null);
        }

        public Filter<DataTableViewModel> FilterOn<T>(object jsOptions)
        {
            var optionsDict = ConvertObjectToDictionary(jsOptions);
            return FilterOn<T>(optionsDict);
        }

        public Filter<DataTableViewModel> FilterOn<T>(IDictionary<string, object> jsOptions)
        {
            return new Filter<DataTableViewModel>(this, FilterTypeRules, (c, t) => t == typeof(T), jsOptions);
        }

        public Filter<DataTableViewModel> FilterOn(string columnName)
        {
            return FilterOn(columnName, null);
        }

        public Filter<DataTableViewModel> FilterOn(string columnName, object jsOptions)
        {
            var optionsDict = ConvertObjectToDictionary(jsOptions);
            return FilterOn(columnName, optionsDict);
        }

        public Filter<DataTableViewModel> FilterOn(string columnName, IDictionary<string, object> jsOptions)
        {
            return new Filter<DataTableViewModel>(this, FilterTypeRules, (c, t) => c == columnName, jsOptions);
        }

        private static string ConvertDictionaryToJsonBody(IDictionary<string, object> dict)
        {
            // Converting to System.Collections.Generic.Dictionary<> to ensure Dictionary will be converted to Json in correct format
            var dictSystem = new Dictionary<string, object>(dict ?? new Dictionary<string, object>());
            return (new JavaScriptSerializer()).Serialize(dictSystem);
        }

        private static IDictionary<string, object> ConvertObjectToDictionary(object obj)
        {
            // Doing this way because RouteValueDictionary converts to Json in wrong format
            return new Dictionary<string, object>(new RouteValueDictionary(obj));
        }

        private void Init(string id, string ajaxUrl, IList<ColDef> columns)
        {
            AjaxUrl = ajaxUrl;
            Id = id;
            Columns = columns;
            FilterTypeRules = new FilterRuleList();
            FilterTypeRules.AddRange(StaticFilterTypeRules);
            ShowSearch = true;
            ShowPageSizes = true;
            Sortable = true;
            TableTools = true;
            AutoWidth = true;
            ControlBoxDataToken = CbDataToken;
            ControlBoxTemplate = ControlBoxTemplateBootstrap;
            ControlBoxTokens = new Dictionary<string, string> { { "#{editTarget}", "~/" }, { "#{deleteTarget}", "~/" } };
            Sort = new[] { new object[] { 0, "asc" } };
            JsOptions = new Dictionary<string, object>();
            CustomData = new Dictionary<string, object>();
            RowIdIndex = null;
            DeferLoading = InitialData != null ? new[] { InitialData.Count() } : null;
            TableToolsButtons = new List<KeyValuePair<string, IDictionary<string, object>>>();
        }

        public class Filter<TTarget>
        {
            private readonly TTarget target;
            private readonly FilterRuleList list;
            private readonly Func<string, Type, bool> predicate;
            private readonly IDictionary<string, object> jsOptions;

            public Filter(TTarget target, FilterRuleList list, Func<string, Type, bool> predicate, IDictionary<string, object> jsOptions)
            {
                this.target = target;
                this.list = list;
                this.predicate = predicate;
                this.jsOptions = jsOptions;
            }

            public TTarget Select(params string[] options)
            {
                var escapedOptions = options.Select(o => o.Replace("'", "\\'"));
                AddRule("{type: 'select', values: ['" + string.Join("','", escapedOptions) + "']}");
                return target;
            }

            public TTarget NumberRange()
            {
                AddRule("{type: 'number-range'}");
                return target;
            }

            public TTarget DateRange()
            {
                AddRule("{type: 'date-range'}");
                return target;
            }

            public TTarget Number()
            {
                AddRule("{type: 'number'}");
                return target;
            }

            public TTarget CheckBoxes(params string[] options)
            {
                var escapedOptions = options.Select(o => o.Replace("'", "\\'"));
                AddRule("{type: 'checkbox', values: ['" + string.Join("','", escapedOptions) + "']}");
                return target;
            }

            public TTarget Text()
            {
                AddRule("{type: 'text'}");
                return target;
            }

            public TTarget None()
            {
                AddRule("null");
                return target;
            }

            private void AddRule(string result)
            {
                if (result != "null" && jsOptions != null && jsOptions.Count > 0)
                {
                    var jsOptionsAsJson = ConvertDictionaryToJsonBody(jsOptions);
                    result = result.TrimEnd('}') + ", " + jsOptionsAsJson + "}";
                }

                list.Insert(0, (c, t) => predicate(c, t) ? result : null);
            }
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class DataTableViewModel<T, TRes> : DataTableViewModel
    {
        public DataTableViewModel(string id, string ajaxUrl, IEnumerable<T> initialData, Func<T, TRes> transform, IList<ColDef> columns)
            : base(id, ajaxUrl, columns)
        {
            InitialData = GetResults(initialData, transform);
            DeferLoading = new[] { InitialData.Count() };
        }

        private static object[][] GetResults(IEnumerable<T> data, Func<T, TRes> transform)
        {
            var filteredData = data.Select(transform).AsQueryable();
            var properties = typeof(TRes).GetSortedProperties();
            return filteredData.Select(i => properties.Select(p => DataTablesParser.GetTransformedValue(p.PropertyType, p.GetGetMethod().Invoke(i, null))).ToArray()).ToArray();
        }
    }
}