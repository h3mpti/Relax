﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Relax.Impl.Configuration;
using Relax.Impl.Http;
using Relax.Impl.Json;
using Symbiote.Core.Extensions;

namespace Relax.Impl.Commands
{
    public class SaveDocumentCommand : BaseCouchCommand
    {
        public virtual CommandResult Save<TModel>(TModel model)
        {
            try
            {
                CreateUri<TModel>()
                    .BulkInsert();

                //var body = model.ToJson(configuration.IncludeTypeSpecification);
                //var result = Put(body);
                //model.SetDocumentRevision(result.JsonObject["rev"].ToString());
                //return result;

                var documents = model.GetDocmentsFromGraph();
                return SaveAll(documents);
            }
            catch (Exception ex)
            {
                throw Exception(
                    ex,
                    "An exception occurred trying to save a document of type {0} at {1}. \r\n\t {2}",
                    typeof(TModel).FullName,
                    Uri.ToString(),
                    ex
                    );
            }
        }

        public virtual CommandResult SaveAll<TModel>(IEnumerable<TModel> models)
        {
            CreateUri<TModel>()
                .BulkInsert();

            var documents = models.GetDocmentsFromGraph();
            return SaveAll(documents);
        }

        public virtual CommandResult SaveAll(string database, IEnumerable<object> models)
        {
            CreateUri(database)
                .BulkInsert();

            var documents = models.GetDocmentsFromGraph();
            return SaveAll(documents);
        }

        protected CommandResult SaveAll(IEnumerable<object> models)
        {
            try
            {
                var list = new BulkPersist(true, false, models);
                var body = list.ToJson(configuration.IncludeTypeSpecification);
                body = ScrubBulkPersistOfTypeTokens(body);

                return Post(body);
            }
            catch (Exception ex)
            {
                throw Exception(
                        ex,
                        "An exception occurred trying to save a collection documents at {0}. \r\n\t {1}",
                        Uri.ToString(),
                        ex
                    );
            }
        }

        public virtual string ScrubBulkPersistOfTypeTokens(string body)
        {
            var jBlob = JObject.Parse(body);

            var hasTypes = jBlob.Children().OfType<JProperty>().FirstOrDefault(x => x.Name == "$type") != null;
            if (hasTypes)
            {
                var allOrNothing = jBlob["all_or_nothing"];
                var nonAtomic = jBlob["non_atomic"];
                var docs = jBlob["docs"]["$values"];

                var newBlob = new JObject(
                    new JProperty("all_or_nothing", allOrNothing),
                    new JProperty("non_atomic", nonAtomic),
                    new JProperty("docs", docs)
                    );

                body = newBlob.ToString();
            }
            return body;
        }

        public SaveDocumentCommand(IHttpAction action, ICouchConfiguration configuration) : base(action, configuration)
        {
        }
    }
}