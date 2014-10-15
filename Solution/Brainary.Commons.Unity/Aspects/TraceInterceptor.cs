namespace Brainary.Commons.Unity.Aspects
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using Microsoft.Practices.Unity.InterceptionExtension;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class TraceInterceptor : ITraceInterceptor
    {
        private readonly ILogger logger;

        public TraceInterceptor(ILogger logger)
        {
            this.logger = logger;
        }

        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(Messages.CallTo, input.Target.GetType().Name, input.MethodBase.Name);
            var pi = new Dictionary<string, object>();
            for (var i = 0; i < input.Arguments.Count; i++) pi.Add(input.Arguments.ParameterName(i), input.Arguments[i]);
            var args = string.Join(", ", pi.Select(s => string.Format("{0}: {1}", s.Key, JsonConvert.SerializeObject(s.Value, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new IsoDateTimeConverter() }, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }))));
            sb.AppendFormat(" {{\r\n{0}\r\n}}\r\n", args);

            var sw = new Stopwatch();
            sw.Start();
            var result = getNext()(input, getNext);
            sw.Stop();
            sb.AppendFormat(Messages.Elapsed, sw.Elapsed.ToString("g"));

            logger.Info(sb.ToString());
            return result;
        }
    }
}
