using AspNetCore.Base.Settings;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Base.ElasticSearch
{
    public static class ElasticSearchExtensions
    {
        //https://www.elastic.co/guide/en/elasticsearch/client/net-api/6.x/nest-getting-started.html
        //https://miroslavpopovic.com/posts/2018/07/elasticsearch-with-aspnet-core-and-docker
        //Index == Table
        //The code will create an index per documentType

        [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
        public sealed class IndexAttribute : Attribute
        {
            public string Name { get; }
            public IndexAttribute(string name)
            {
                if(string.IsNullOrWhiteSpace(name))
                {
                    throw new Exception("Index name cannot be empty");
                }

                Name = name;
            }
        }

        public static void AddElasticSearch(this IServiceCollection services, string url, string defaultIndex = "default", string defaultTypeName = "doc")
        {
            var settings = new ConnectionSettings(new Uri(url))
                .PrettyJson()
                .BasicAuthentication("elastic","elastic")
                .DefaultIndex(defaultIndex)
                .DefaultTypeName(defaultTypeName);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
        }

        //https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker
        public static LoggerConfiguration AddElasticSearchLogging(this LoggerConfiguration loggerConfiguration, IConfiguration configuration)
        {
            var elasticSettingsSection = configuration.GetChildren().FirstOrDefault(item => item.Key == "ElasticSettings");
            if (elasticSettingsSection != null)
            {
                var elasticSettings = elasticSettingsSection.Get<ElasticSettings>();

                if (elasticSettings.Log && string.IsNullOrWhiteSpace(elasticSettings.Uri))
                {
                    loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSettings.Uri))
                    {
                        AutoRegisterTemplate = true,
                    });
                }
            }

            return loggerConfiguration;
        }

        private static string GetIndex<TDocument>(string indexSuffix = null)
        {
            var type = typeof(TDocument);
            var indexAttribute = type.GetCustomAttributes(typeof(IndexAttribute), false).Select(a => (IndexAttribute)a).FirstOrDefault();
            var index = (indexAttribute != null ? indexAttribute.Name : typeof(TDocument).Name.ToLower()) + (string.IsNullOrWhiteSpace(indexSuffix) ? "" : $"-{indexSuffix}");
            return index;
        }

        public static Task<IIndexResponse> IndexAsync<TDocument>(
            this IElasticClient client, 
            TDocument document, 
            CancellationToken cancellationToken = default(CancellationToken), 
            string indexSuffix = null)
            where TDocument : class
        {
           var index = GetIndex<TDocument>(indexSuffix);
           return client.IndexAsync(
               document, 
               s => s.Index(index), 
               cancellationToken);
        }

        public static Task<IUpdateResponse<TDocument>> UpsertAsync<TDocument>(
            this IElasticClient client, 
            TDocument document, 
            CancellationToken cancellationToken = default(CancellationToken), 
            string indexSuffix = null)
           where TDocument : class
        {
            var index = GetIndex<TDocument>(indexSuffix);
            return client.UpdateAsync<TDocument>(
                document, 
                u => u.Index(index).Doc(document).DocAsUpsert(true), 
                cancellationToken);
        }

        public static Task<ISearchResponse<TDocument>> SearchAsync<TDocument>(
            this IElasticClient client, 
            Func<SearchDescriptor<TDocument>, SearchDescriptor<TDocument>> selector, 
            CancellationToken cancellationToken = default(CancellationToken), 
            string indexSuffix = null)
           where TDocument : class
        {
            var index = GetIndex<TDocument>(indexSuffix);
            return client.SearchAsync<TDocument>(
                s => selector(s.Index(index)), 
                cancellationToken);
        }

        public static Task<IDeleteByQueryResponse> DeleteByQueryAsync<TDocument>(
            this IElasticClient client, 
            Func<DeleteByQueryDescriptor<TDocument>, DeleteByQueryDescriptor<TDocument>> selector, 
            CancellationToken cancellationToken = default(CancellationToken), 
            string indexSuffix = null)
           where TDocument : class
        {
            var index = GetIndex<TDocument>(indexSuffix);
            return client.DeleteByQueryAsync<TDocument>(
                s => selector(s.Index(index)), 
                cancellationToken);
        }

        public static Task<IDeleteIndexResponse> DeleteIndexAsync<TDocument>(
            this IElasticClient client, CancellationToken cancellationToken = default(CancellationToken), 
            string indexSuffix = null)
            where TDocument : class
        {
            var index = GetIndex<TDocument>(indexSuffix);
            return client.DeleteIndexAsync(
                index, 
                null, 
                cancellationToken);
        }

        public static Task<IDeleteIndexResponse> DeleteIndexAsync(
            this IElasticClient client, 
            string index, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return client.DeleteIndexAsync(
                Indices.Index(index), 
                null, 
                cancellationToken
                );
        }

        //https://rimdev.io/bulk-import-documents-into-elasticsearch-using-nest/
        public static void BulkAll<TDocument>(
            this IElasticClient client, 
            IEnumerable<TDocument> documents, 
            CancellationToken cancellationToken = default(CancellationToken), 
            string indexSuffix = null)
             where TDocument : class
        {
            var index = GetIndex<TDocument>(indexSuffix);
            var waitHandle = new CountdownEvent(1);

            var bulkAll = client.BulkAll(documents, b => b
                .Index(index)
                .BackOffRetries(2)
                .BackOffTime("30s")
                .RefreshOnCompleted(true)
                .MaxDegreeOfParallelism(4)
                .Size(1000),
                cancellationToken
            );

            bulkAll.Subscribe(new BulkAllObserver(
                onNext: (b) => { Console.Write("."); },
                onError: (e) => { throw e; },
                onCompleted: () => waitHandle.Signal()
            ));

            waitHandle.Wait();
        }
    }
}
